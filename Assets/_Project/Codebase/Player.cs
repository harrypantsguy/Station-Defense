using FishingGame.Utilities;
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
        
        public StructureName structureName;
        public ToolType toolType;

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

        public void SetStructure(StructureName structureName)
        {
            this.structureName = structureName;
        }

        private void Update()
        {
            Vector2 worldMousePos = Utils.WorldMousePos;

            //Debug.Log($"{Station.Singleton.WorldToGridPos2D(worldMousePos)}");

            _placementImage.transform.position = Station.Singleton.SnapPointToGrid(worldMousePos);
            _placementImage.rectTransform.sizeDelta = Vector2.one;

            _structureSelectionImage.sprite = References.Singleton.structureSpriteDictionary[structureName];
            
            Structure newStructure = Structure.GetStructureFromType(structureName);

            bool isValidPlacement = toolType == ToolType.Single &&
                                    Station.Singleton.IsValidPlacementAtWorldPos(worldMousePos) ||
                                    toolType == ToolType.Rect &&
                                    Station.Singleton.IsRectViableToFill(_rectStartPos,
                                        worldMousePos, newStructure);
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
            
            if (toolType == ToolType.Single && !mouseOverUI)
            {
                if (GameControls.PlaceStructure.IsHeld)
                {
                    Station.Singleton.TrySetGridPosStructure(worldMousePos, newStructure);
                }
            }
            else if (toolType == ToolType.Rect)
            {
                if (GameControls.PlaceStructure.IsPressed && !mouseOverUI)
                {
                    _drawingRect = true;
                    _rectStartPos = worldMousePos;
                }
                else if (GameControls.PlaceStructure.IsReleased)
                {
                    if (_drawingRect && !mouseOverUI)
                        Station.Singleton.FillRectWithStructure(_rectStartPos, worldMousePos, newStructure);
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
