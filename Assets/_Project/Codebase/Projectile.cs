using UnityEngine;

namespace _Project.Codebase
{
    public class Projectile : MonoBehaviour
    {
        public float radius;
        public int tileSplash;
        private bool queuedForDestruction;

        private const float SPEED = 40f;
        private void FixedUpdate()
        {
            if (queuedForDestruction)
            {
                Destroy(gameObject);
                return;
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
                    Debug.DrawRay(hit.point, hit.normal * .1f, Color.yellow, .25f);
                    if (station.TakeDamage(hit.point - hit.normal * .05f, tileSplash))
                    {
                        queuedForDestruction = true;
                        transform.position = hit.point + hit.normal * radius;
                        return;
                    }
                }
            }
            
            transform.position = newPosition;
        }
    }
}