using DanonsTools.Plugins.DanonsTools.Utilities;
using UnityEngine;

namespace _Project.Codebase
{
    public class Turret : Structure
    {
        [SerializeField] private GameObject _turretBarrelObj;
        [SerializeField] private Transform _projectileSpawnPos;
        [SerializeField] private GameObject _projectilePrefab;
        
        public Vector2 target;

        private float _lastFireTime;
        
        private const float FIRE_DELAY = .5f;

        protected override void Start()
        {
            base.Start();

            _lastFireTime = Time.time;
        }

        protected override void Update()
        {
            base.Update();

            target = Utils.WorldMousePos;
            
            if (Built)
            {
                _turretBarrelObj.transform.right = (target - (Vector2) transform.position).normalized;
            }

            if (GameControls.FireDefenses.IsHeld && Time.time > _lastFireTime + FIRE_DELAY)
            {
                _lastFireTime = Time.time;
                Projectile.FireProjectile(_projectilePrefab, _projectileSpawnPos.position, target, Layers.EnemyMask);
            }
        }
    }
}