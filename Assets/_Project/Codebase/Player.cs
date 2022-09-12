using System.Collections.Generic;
using DanonsTools.Plugins.DanonsTools.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Codebase
{
    public class Player : MonoSingleton<Player>
    {
        [SerializeField] private GameObject _projectilePrefab;
        [SerializeField] private Image _placementImage;
        [SerializeField] private TMP_Text _xText; 
        [SerializeField] private TMP_Text _yText;
        [SerializeField] private Image _structureSelectionImage;
        [SerializeField] private Toggle _destroyFloorsToggle;
        [SerializeField] private Toggle _fillToggle;
        
        public PlaceableName placeableName;
        public ToolType toolType;

        private bool _fillRect;
        private bool _destroyFloors;
        private bool _drawingRect;
        private Vector2 _rectStartPos;
        private Vector2Int _rectSize;
        
        private float _lastFireTime;
        private Camera _cam;

        private void Start()
        {
            _lastFireTime = Time.time;
            _cam = CameraController.Singleton.camera;
       }

        public void SetStructure(PlaceableName placeableName) => this.placeableName = placeableName;
        public void SetDestroyFloorsState(bool state) => _destroyFloors = state;

        private void Update()
        {
            _fillRect = _fillToggle.isOn;
            _destroyFloors = _destroyFloorsToggle.isOn;  
            Vector2 worldMousePos = Utils.WorldMousePos;

            Vector2Int mouseGridPos = Station.Singleton.WorldToGridPos2D(worldMousePos);
            
            //Debug.Log($"{mouseGridPos} " +
            //          (Station.Singleton.TryGetFloorAtGridPos(mouseGridPos, out FloorTile floorTile)
            //              ? floorTile.Built.ToString() : "Unbuilt"));

            _placementImage.transform.position = Station.Singleton.SnapPointToGrid(worldMousePos);
            _placementImage.rectTransform.sizeDelta = Vector2.one;

            _structureSelectionImage.sprite = References.Singleton.GetSprite(placeableName);

            TileConstruct newTileConstruct = TileConstruct.GetTileConstructFromName(placeableName);

            bool isValidPlacement = newTileConstruct == null || toolType == ToolType.Single && 
                                                                     newTileConstruct.IsValidPlacementAtGridPos(
                                                                         Station.Singleton, mouseGridPos) ||
                                    toolType == ToolType.Rect &&
                                    newTileConstruct.IsValidRectPlacement(Station.Singleton, 
                                        Station.Singleton.WorldToGridPos2D(_rectStartPos),
                                        mouseGridPos, !_fillRect, true, out List<Vector2Int> pos);
            bool mouseOverUI = CustomUI.MouseOverUI;

            if (_drawingRect)
            {
                _xText.enabled = true;
                _yText.enabled = true;
                _xText.text = _rectSize.x.ToString();
                _yText.text = _rectSize.y.ToString();
            }
            else
            {
                _xText.enabled = false;
                _yText.enabled = false;
            }

            _placementImage.color = isValidPlacement ? Color.white : Color.red;
            _placementImage.enabled = _drawingRect || !mouseOverUI;
            
            if (toolType == ToolType.Single && !mouseOverUI && isValidPlacement)
            {
                if (GameControls.PlaceStructure.IsHeld)
                {
                    if (newTileConstruct != null) 
                        newTileConstruct.TryPlace(Station.Singleton, Station.Singleton.WorldToGridPos2D(worldMousePos));
                    else
                    {
                        if (_destroyFloors)
                            Station.Singleton.TryRemoveFloorAtGridPos(mouseGridPos);
                        else if (Station.Singleton.TryGetPlaceableAtGridPos(mouseGridPos, out IPlaceable placeable))
                        {
                            placeable.Delete(Station.Singleton);
                        }
                    }
                }
            }
            else if (toolType == ToolType.Rect)
            {
                if (GameControls.CancelSelection.IsPressed)
                {
                    _drawingRect = false;
                }
                
                if (GameControls.PlaceStructure.IsPressed && !mouseOverUI)
                {
                    _drawingRect = true;
                    _rectStartPos = worldMousePos;
                }
                else if (GameControls.PlaceStructure.IsReleased)
                {
                    if (_drawingRect)
                    {
                        Vector2Int start = Station.Singleton.WorldToGridPos2D(_rectStartPos);
                        if (newTileConstruct != null)
                            newTileConstruct.TryFillRect(Station.Singleton, start, mouseGridPos, !_fillRect);
                        else
                        {
                            Station.Singleton.RemoveAllOfTypeInRect(start, mouseGridPos, !_fillRect,
                                _destroyFloors ? ConstructType.Floor : ConstructType.Wall);
                        }
                    }
                    
                    _drawingRect = false;
                }
                else if (!GameControls.PlaceStructure.IsHeld)
                    _rectStartPos = worldMousePos;

                if (_drawingRect)
                {
                    Vector2 startGridPos = Station.Singleton.SnapPointToGrid(_rectStartPos);
                    Vector2 endGridPos = Station.Singleton.SnapPointToGrid(worldMousePos);

                    Vector2 displacement = endGridPos - startGridPos;
                    
                    _rectSize = new Vector2Int((int)Mathf.Abs(displacement.x) + 1, (int)Mathf.Abs(displacement.y) + 1);
                    _placementImage.transform.position = displacement / 2f + startGridPos;
                    
                    _placementImage.rectTransform.sizeDelta = _rectSize;

                }
            }
            
            if (GameControls.FireDefenses.IsHeld && Time.time > _lastFireTime + .075f)
            {
                _lastFireTime = Time.time;
                
                GameObject projectile = Instantiate(_projectilePrefab);
                projectile.transform.position = Utils.WorldMousePos;
                projectile.transform.right = -projectile.transform.position;
            }
        }
    }
}
