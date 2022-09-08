using UnityEngine;

namespace _Project.Codebase
{
    public class Projectile : MonoBehaviour
    {
        private bool queuedForDestruction;
        private void FixedUpdate()
        {
            if (queuedForDestruction)
            {
                Destroy(gameObject);
                return;
            }

            Vector2 velocity = transform.right * (3f * Time.fixedDeltaTime);
            Vector2 newPosition = (Vector2)transform.position + velocity;

            RaycastHit2D hit = Physics2D.CircleCast(transform.position, .25f, velocity);

            if (hit.collider != null)
            {
                queuedForDestruction = true;
                transform.position = hit.point;
                return;
            }
            
            transform.position = newPosition;
        }
    }
}