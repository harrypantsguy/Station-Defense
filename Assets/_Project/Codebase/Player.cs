using System;
using System.Collections.Generic;
using System.Diagnostics;
using DanonsTools.Plugins.DanonsTools.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

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

        public PlaceableName PlaceableName { get; private set; }
        public ToolType ToolType { get; private set; }

        private PlaceableType _placeableType;
        private Structure _newPlaceableAsStructure;
        private Direction _placementDir;
        private bool _fillRect;
        private bool _destroyFloors;
        private bool _drawingRect;
        private Vector2 _rectStartPos;
        private Vector2Int _rectSize;
        
        private float _lastFireTime;
        private Camera _cam;
        private IPlaceable _newPlaceable;

        private void Start()
        {
            _lastFireTime = Time.time;
            _cam = CameraController.Singleton.camera;

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
            
            _placeableType = References.Singleton.GetType(PlaceableName);
            
            if (_placeableType == PlaceableType.Structure)
                _newPlaceableAsStructure = _newPlaceable as Structure;
        }

        private void Update()
        {
            if (GameControls.RotateStructureLeft.IsPressed)
                _placementDir = _placementDir.Rotate(-1);
            if (GameControls.RotateStructureRight.IsPressed)
                _placementDir = _placementDir.Rotate(1);

            _fillRect = _fillToggle.isOn;
            _destroyFloors = _destroyFloorsToggle.isOn;  
            Vector2 worldMousePos = Utils.WorldMousePos;

            Vector2Int mouseGridPos = Station.Singleton.WorldToGridPos2D(worldMousePos);
            
            _placementImage.rectTransform.sizeDelta = Vector2.one;
            _placementImage.transform.position = Station.Singleton.GridToWorldPos(mouseGridPos);
            
            if (_placeableType == PlaceableType.Structure)
            {
                _newPlaceableAsStructure.Direction = _placementDir;
                if (_newPlaceableAsStructure.xEven && _newPlaceableAsStructure.yEven)
                {
                    Vector2 modPos = new Vector2(worldMousePos.x % 1f, worldMousePos.y % 1f);
                    int xOffset = (worldMousePos.x < 0f ? 1f - modPos.x.Abs() : modPos.x) > .5f ? 1 : 0;
                    int yOffset = (worldMousePos.y < 0f ? 1f - modPos.y.Abs() : modPos.y) > .5f ? 1 : 0;

                    _placementImage.transform.position = _newPlaceableAsStructure.transform.position;
                    mouseGridPos += new Vector2Int(xOffset, yOffset);
                }

                _placementImage.rectTransform.sizeDelta = Vector2.zero; //_newPlaceableAsStructure.Dimensions;

                //Debug.Log($"sizeDelta: {_placementImage.rectTransform.sizeDelta}");
            }

            //Debug.Log($"{mouseGridPos} " +
            //          (Station.Singleton.TryGetFloorAtGridPos(mouseGridPos, out FloorTile floorTile)
            //              ? floorTile.Built.ToString() : "Unbuilt"));

            _structureSelectionImage.sprite = References.Singleton.GetSprite(PlaceableName);
            _structureSelectionImage.preserveAspect = true;
            
            bool isValidPlacement = _newPlaceable == null || ToolType == ToolType.Single && 
                                    _newPlaceable.IsValidPlacementAtGridPos(
                                        Station.Singleton, mouseGridPos) ||
                                    ToolType == ToolType.Rect &&
                                    _newPlaceable.IsValidRectPlacement(Station.Singleton, 
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
            if (_placeableType == PlaceableType.Structure)
                _newPlaceableAsStructure.gameObject.SetActive(isValidPlacement);

            if (ToolType == ToolType.Single && !mouseOverUI && isValidPlacement)
            {
                if (GameControls.PlaceStructure.IsHeld)
                {
                    if (_newPlaceable != null) 
                    {
                        _newPlaceable.TryPlace(Station.Singleton, mouseGridPos);
                        _newPlaceable = null;
                        GenerateNewPlaceable();
                    }
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
                else if (_placeableType == PlaceableType.Structure)
                {
                    _newPlaceableAsStructure.SetGridPosition(Station.Singleton, mouseGridPos);
                }
            }
            else if (ToolType == ToolType.Rect)
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
                        if (_newPlaceable != null)
                        {
                            _newPlaceable.TryFillRect(Station.Singleton, start, mouseGridPos, !_fillRect);
                            GenerateNewPlaceable();
                        }
                        else
                        {
                            Station.Singleton.RemoveAllOfTypeInRect(start, mouseGridPos, !_fillRect,
                                _destroyFloors ? PlaceableType.Floor : PlaceableType.Wall);
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
    }
}
