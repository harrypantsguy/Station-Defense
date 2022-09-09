using System;
using FishingGame.Utilities;
using UnityEngine;

namespace _Project.Codebase
{
    public class Player : MonoSingleton<Player>
    {
        [SerializeField] private GameObject _projectilePrefab;
        public StructureType structureType;

        private float _lastFireTime;

        private void Start()
        {
            _lastFireTime = Time.time;
        }


        public void SetStructure(StructureType type)
        {
            structureType = type;
        }

        private void Update()
        {
            if (CustomUI.MouseOverUI) return;
            
            if (GameControls.PlaceStructure.IsHeld)
            {
                Station.Singleton.PlaceStructure(Utils.WorldMousePos, Structure.GetStructureFromType(structureType));
            }
            else if (GameControls.RemoveStructure.IsHeld)
            {
                Station.Singleton.RemoveStructure(Utils.WorldMousePos);
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
