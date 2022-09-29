using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace _Project.Codebase
{
    public class PlacementRectUI : CustomUI
    {
        [SerializeField] private TMP_Text _costText;
        [SerializeField] private TMP_Text _xText; 
        [SerializeField] private TMP_Text _yText;
        
        public override bool FlagWhenMouseOver => false;

        private Image _placementImage;
        private Player _player;
        private CameraController _camController;

        protected override void Start()
        {
            base.Start();
            _placementImage = GetComponent<Image>();
            _player = Player.Singleton;
            _camController = CameraController.Singleton;
        }

        private void LateUpdate()
        {
            Vector2 worldPosition = _player.GridSnappedMousePos;
            Vector2 _worldRectSize = Vector2Int.one;
            
            if (_player.PlaceableType == PlaceableType.Structure)
            {
                /*
                //_newPlaceableAsStructure.Direction = _placementDir;
                if (_newPlaceableAsStructure.xEven && _newPlaceableAsStructure.yEven)
                {
                    Vector2 modPos = new Vector2(worldMousePos.x % 1f, worldMousePos.y % 1f);
                    int xOffset = (worldMousePos.x < 0f ? 1f - modPos.x.Abs() : modPos.x) > .5f ? 1 : 0;
                    int yOffset = (worldMousePos.y < 0f ? 1f - modPos.y.Abs() : modPos.y) > .5f ? 1 : 0;

                    _placementImage.transform.position = _newPlaceableAsStructure.transform.position;
                    mouseGridPos += new Vector2Int(xOffset, yOffset);
                }
                */

                _worldRectSize = Vector2.zero; //_newPlaceableAsStructure.Dimensions;

                //Debug.Log($"sizeDelta: {_placementImage.rectTransform.sizeDelta}");
            }

            
            if (_player.DrawingRect)
            {
                Vector2 startGridPos = Station.Singleton.SnapPointToGrid(_player.RectStartPos);
                Vector2 endGridPos = _player.GridSnappedMousePos;

                Vector2 displacement = endGridPos - startGridPos;
                    
                _worldRectSize = new Vector2Int((int)Mathf.Abs(displacement.x) + 1, (int)Mathf.Abs(displacement.y) + 1);
                worldPosition = displacement / 2f + startGridPos;
            }

            rectTransform.sizeDelta = _worldRectSize;// * (_camController.ScreenSpaceCanvas.referencePixelsPerUnit
                                                     //   * 5f / _camController.Camera.orthographicSize);
            transform.position = worldPosition;//_camController.Camera.WorldToScreenPoint(worldPosition);
            
            _costText.text = _player.PlacementCost.credits.ToString();
            _costText.color = _player.HasValidPlacement ? Color.white : Color.red;
            _costText.enabled = _player.PlaceableName != PlaceableName.None;
            Debug.Log(_player.PlacementFailCause.ToString());
            
            if (_player.DrawingRect)
            {
                _xText.enabled = true;
                _yText.enabled = true;
                _xText.text = _worldRectSize.x.ToString();
                _yText.text = _worldRectSize.y.ToString();
            }
            else
            {
                _xText.enabled = false;
                _yText.enabled = false;
            }

            _placementImage.color = _player.HasValidPlacement ? Color.white : Color.red;
            _placementImage.enabled = _player.DrawingRect || !MouseOverUI;
            //_placementImage.pixelsPerUnitMultiplier = 80f * (5f / _camController.Camera.orthographicSize);
        }
    }
}