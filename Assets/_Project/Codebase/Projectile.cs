using System.Runtime.InteropServices;
using UnityEngine;

namespace _Project.Codebase
{
    public class Projectile : MonoBehaviour
    {
        public float radius;
        public float splashRange;

        private LayerMask _hitMask;

        private bool _queuedForDestruction;
        private float _creationTime;
        private Vector2 _hitTarget;
        private Station _station;
        private const float MAX_LIFETIME = 5f;

        private void Start()
        {
            _station = Station.Singleton;
            _creationTime = Time.time;
        }

        private const float SPEED = 40f;
        private void FixedUpdate()
        {
            if (_queuedForDestruction)
            {
                Destroy(gameObject);
                return;
            }

            /*
            if (station.IsPlaceableAtGridPos(transform.position))
            {
                TryHitStation(transform.position);
            }
            */
            
            float distance = (SPEED * Time.fixedDeltaTime);
            Vector2 velocity = transform.right * distance;
            Vector2 newPosition = (Vector2)transform.position + velocity;

            RaycastHit2D hit = Physics2D.CircleCast(transform.position, radius, transform.right, 
                distance, _hitMask);

            if (hit.collider != null)
            {
                Hit(hit.point);
            }

            if (Vector2.Distance(newPosition, _hitTarget) < .25f)
            {
                Hit(_hitTarget);
            }

            transform.position = newPosition;
        }

        private void Update()
        {
            if (Time.time > _creationTime + MAX_LIFETIME)
                Destroy(gameObject);
        }

        public void Hit(Vector2 pos)
        {
            Explosion explosion = new Explosion(pos, splashRange);
            _queuedForDestruction = true;
        }

        public static Projectile FireProjectile(GameObject projectilePrefab, Vector2 spawnPoint, Vector2 target, 
            LayerMask hitmask)
        {
            Projectile projectile = Instantiate(projectilePrefab).GetComponent<Projectile>();
            projectile._hitMask = hitmask;
            projectile.transform.position = spawnPoint;
            projectile._hitTarget = target;
            projectile.transform.right = (target - spawnPoint).normalized;
            return projectile;
        }
    }
}