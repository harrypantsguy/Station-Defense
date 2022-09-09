using System;
using UnityEngine;

namespace _Project.Codebase
{
    public class Projectile : MonoBehaviour
    {
        public float radius;
        public int tileSplash;
        private bool queuedForDestruction;

        private Station station;

        private void Start()
        {
            station = Station.Singleton;
        }

        private const float SPEED = 40f;
        private void FixedUpdate()
        {
            if (queuedForDestruction)
            {
                Destroy(gameObject);
                return;
            }

            if (station.IsStructureAtWorldPos(transform.position))
            {
                TryHitStation(transform.position);
            }
            
            float distance = (SPEED * Time.fixedDeltaTime);
            Vector2 velocity = transform.right * distance;
            Vector2 newPosition = (Vector2)transform.position + velocity;

            RaycastHit2D hit = Physics2D.CircleCast(transform.position, radius, transform.right, 
                distance);

            if (hit.collider != null)
            {
                if (hit.transform.parent != null && hit.transform.parent.parent != null
                                                 && hit.transform.parent.parent.TryGetComponent(out Station station))
                {
                    if (TryHitStation(hit.point - hit.normal * .05f))
                        return;
                    Debug.DrawRay(hit.point, hit.normal * .1f, Color.yellow, .25f);
                }
            }
            
            transform.position = newPosition;
        }

        private bool TryHitStation(Vector2 pos)
        {
            if (station.TakeDamage(pos, tileSplash))
            {
                queuedForDestruction = true;
                transform.position = pos;
                return true;
            }

            return false;
        }
    }
}