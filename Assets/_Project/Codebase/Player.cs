using FishingGame.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Codebase
{
    public class Player : MonoSingleton<Player>
    {
        [SerializeField] private GameObject _projectilePrefab;
        [SerializeField] private Image _placementImage;
        
        public StructureType structureType;

        private float _lastFireTime;
        private Camera _cam;

        private void Start()
        {
            _lastFireTime = Time.time;
            _cam = CameraController.Singleton.camera;
        }

        public void SetStructure(StructureType type)
        {
            structureType = type;
        }

        private void Update()
        {
            Vector2 worldMousePos = Utils.WorldMousePos;

            //_placementImage.transform.position =
            //    _cam.WorldToScreenPoint();

            _placementImage.transform.position = Station.Singleton.SnapPointToGrid(worldMousePos);

            bool isValidPlacement = Station.Singleton.IsValidPlacementAtWorldPos(worldMousePos);

            _placementImage.color = isValidPlacement ? Color.white : Color.red;
            _placementImage.enabled = !CustomUI.MouseOverUI;
            
            Debug.Log($"{Station.Singleton.WorldToGridPos2D(worldMousePos)}");

            if (CustomUI.MouseOverUI) return;
            
            if (GameControls.PlaceStructure.IsHeld && isValidPlacement)
            {
                Station.Singleton.PlaceStructure(worldMousePos, Structure.GetStructureFromType(structureType));
            }
            else if (GameControls.RemoveStructure.IsHeld)
            {
                Station.Singleton.RemoveStructure(worldMousePos);
            }

            if (GameControls.FireDefenses.IsHeld && Time.time > _lastFireTime + .05f)
            {
                _lastFireTime = Time.time;
                
                GameObject projectile = Instantiate(_projectilePrefab);
                projectile.transform.position = Utils.WorldMousePos;
                projectile.transform.right = -projectile.transform.position;
            }
        }
    }
}
