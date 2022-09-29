using DanonsTools.Plugins.DanonsTools.Utilities;
using UnityEngine;

namespace _Project.Codebase
{
    public class Turret : Structure
    {
        [SerializeField] private GameObject _turretBarrelObj;
        [SerializeField] private Transform _projectileSpawnPos;
        [SerializeField] private GameObject _projectilePrefab;

        protected override void Update()
        {
            base.Update();

            if (Built)
            {
                _turretBarrelObj.transform.right = (Utils.WorldMousePos - (Vector2) transform.position).normalized;
            }

            if (GameControls.FireDefenses.IsPressed)
            {
                GameObject newProjectile = Instantiate(_projectilePrefab);
                newProjectile.transform.position = _projectileSpawnPos.position;
                newProjectile.transform.right = _turretBarrelObj.transform.right;
            }
        }
    }
}