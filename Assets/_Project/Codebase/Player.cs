using System.Collections.Generic;
using DanonsTools.Plugins.DanonsTools.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Codebase
{
    public class Player : MonoSingleton<Player>
    {
        [SerializeField] private GameObject _projectilePrefab;
        [SerializeField] private Image _structureSelectionImage;
        [SerializeField] private Toggle _destroyFloorsToggle;
        [SerializeField] private Toggle _fillToggle;

        public ResourcesContainer resources;
        public PlaceableName PlaceableName { get; private set; }
        public ToolType ToolType { get; private set; }
        public PlaceableType PlaceableType { get; private set; }
        public bool DrawingRect { get; private set; }
        public bool StructureIsEven { get; private set; }
        public bool HasValidPlacement { get; private set; }
        public ResourcesContainer PlacementCost => _placementCost;
        public PlacementFailCause PlacementFailCause { get; private set; }
        public Vector2 WorldMousePos { get; private set; }
        public Vector2Int MouseGridPos { get; private set; }
        public Vector2 GridSnappedMousePos { get; private set; }
        public Vector2 RectStartPos { get; private set; }

        private Structure _newPlaceableAsStructure;
        private Direction _placementDir;
        private bool _fillRect;
        private bool _destroyFloors;
        private ResourcesContainer _placementCost;
        
        private float _lastFireTime;
        private Camera _cam;
        private IPlaceable _newPlaceable;

        private void Start()
        {
            _lastFireTime = Time.time;
            _cam = CameraController.Singleton.Camera;
            resources = new ResourcesContainer(9999);
            
            GenerateNewPlaceable();
        }

        public void SetStructure(PlaceableName placeableName)
        {
            PlaceableName = placeableName;
            GenerateNewPlaceable();
        }

        public void SetToolType(ToolType tool) => ToolType = tool;
        public void SetDestroyFloorsState(bool state) => _destroyFloors = state;

        private void GenerateNewPlaceable()
        {
            if (_newPlaceable != null && _newPlaceable is Structure structure)
            {
                Destroy(structure.gameObject);
            }
            _newPlaceable = IPlaceable.CreatePlaceableFromName(PlaceableName);
            
            PlaceableType = References.Singleton.GetType(PlaceableName);
            
            if (PlaceableType == PlaceableType.Structure)
                _newPlaceableAsStructure = _newPlaceable as Structure;
        }

        private void Update()
        {
            if (GameControls.RotateStructureLeft.IsPressed)
                _placementDir = _placementDir.Rotate(-1);
            if (GameControls.RotateStructureRight.IsPressed)
                _placementDir = _placementDir.Rotate(1);
            
            bool mouseOverUI = CustomUI.MouseOverUI;
            
            _fillRect = _fillToggle.isOn;
            _destroyFloors = _destroyFloorsToggle.isOn;  
            WorldMousePos = Utils.WorldMousePos;

            MouseGridPos = Station.Singleton.WorldToGridPos2D(WorldMousePos);

            if (PlaceableType == PlaceableType.Structure)
            {
                _newPlaceableAsStructure.Direction = _placementDir;
                StructureIsEven = _newPlaceableAsStructure.xEven && _newPlaceableAsStructure.yEven;
                
                if (StructureIsEven)
                {
                    Vector2 modPos = new Vector2(WorldMousePos.x % 1f, WorldMousePos.y % 1f);
                    int xOffset = (WorldMousePos.x < 0f ? 1f - modPos.x.Abs() : modPos.x) > .5f ? 1 : 0;
                    int yOffset = (WorldMousePos.y < 0f ? 1f - modPos.y.Abs() : modPos.y) > .5f ? 1 : 0;

                    //_placementImage.transform.position = _newPlaceableAsStructure.transform.position;
                    MouseGridPos += new Vector2Int(xOffset, yOffset);
                }

                //_placementImage.rectTransform.sizeDelta = Vector2.zero; //_newPlaceableAsStructure.Dimensions;

                //Debug.Log($"sizeDelta: {_placementImage.rectTransform.sizeDelta}");
            }
            
            GridSnappedMousePos = Station.Singleton.GridToWorldPos(MouseGridPos);

            //Debug.Log($"{mouseGridPos} " +
            //          (Station.Singleton.TryGetFloorAtGridPos(mouseGridPos, out FloorTile floorTile)
            //              ? floorTile.Built.ToString() : "Unbuilt"));

            if (ToolType == ToolType.Rect)
            {
                if (GameControls.CancelSelection.IsPressed)
                {
                    DrawingRect = false;
                }
                
                if (GameControls.PlaceStructure.IsPressed && !mouseOverUI)
                {
                    DrawingRect = true;
                    RectStartPos = WorldMousePos;
                }
                else if (GameControls.PlaceStructure.IsReleased)
                {
                    if (DrawingRect)
                    {
                        Vector2Int start = Station.Singleton.WorldToGridPos2D(RectStartPos);
                        if (_newPlaceable != null)
                        {
                            _newPlaceable.TryFillRect(Station.Singleton, start, MouseGridPos, !_fillRect);
                            GenerateNewPlaceable();
                        }
                        else
                        {
                            Station.Singleton.RemoveAllOfTypeInRect(start, MouseGridPos, !_fillRect,
                                _destroyFloors ? PlaceableType.Floor : PlaceableType.Wall);
                        }
                    }
                    
                    DrawingRect = false;
                }
                else if (!DrawingRect)
                    RectStartPos = WorldMousePos;
            }
            
            _structureSelectionImage.sprite = References.Singleton.GetSprite(PlaceableName);
            _structureSelectionImage.preserveAspect = true;
            
            PlacementFailCause failCause = PlacementFailCause.None;
            
            HasValidPlacement = _newPlaceable == null || ToolType == ToolType.Single && 
                                _newPlaceable.IsValidPlacementAtGridPos(
                                    Station.Singleton, MouseGridPos, true, out _placementCost, out failCause) ||
                                ToolType == ToolType.Rect &&
                                _newPlaceable.IsValidRectPlacement(Station.Singleton, 
                                    Station.Singleton.WorldToGridPos2D(RectStartPos),
                                    MouseGridPos, !_fillRect, false, out List<Vector2Int> pos, 
                                    true, out _placementCost, out failCause);

            PlacementFailCause = failCause;
            
            if (PlaceableType == PlaceableType.Structure)
                _newPlaceableAsStructure.gameObject.SetActive(HasValidPlacement);

            if (ToolType == ToolType.Single && !mouseOverUI && HasValidPlacement)
            {
                if (GameControls.PlaceStructure.IsHeld)
                {
                    if (_newPlaceable != null) 
                    {
                        _newPlaceable.TryPlace(Station.Singleton, MouseGridPos);
                        _newPlaceable = null;
                        GenerateNewPlaceable();
                    }
                    else
                    {
                        if (_destroyFloors)
                            Station.Singleton.TryRemoveFloorAtGridPos(MouseGridPos);
                        else if (Station.Singleton.TryGetPlaceableAtGridPos(MouseGridPos, out IPlaceable placeable))
                        {
                            placeable.Delete();
                        }
                    }
                }
                else if (PlaceableType == PlaceableType.Structure)
                {
                    _newPlaceableAsStructure.SetGridPosition(Station.Singleton, MouseGridPos);
                }
            }

            /*
            if (GameControls.FireDefenses.IsHeld && Time.time > _lastFireTime + .075f)
            {
                _lastFireTime = Time.time;
                
                GameObject projectile = Instantiate(_projectilePrefab);
                projectile.transform.position = Utils.WorldMousePos;
                projectile.transform.right = -projectile.transform.position;
            }
            */
        }

        public static bool CanAfford(ResourcesContainer cost) => Singleton.resources >= cost;
    }
}
