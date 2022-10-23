using UnityEditor;
using UnityEngine;

namespace _Project.Codebase
{
    public class Explosion
    {
        public Vector2 location;
        public float size;

        public Explosion(Vector2 pos, float size)
        {
            location = pos;
            this.size = size;
            Vector2Int gridPos = Station.Singleton.WorldToGridPos2D(pos);
            int tileSplash = Mathf.FloorToInt(size);

            Collider2D[] hits = Physics2D.OverlapCircleAll(pos, size);
            foreach (Collider2D hit in hits)
            {
                if (hit.TryGetComponent(out SpacecraftPart shipPart))
                {
                    shipPart.TakeDamage(new DamageReport(50));
                }
            }

            for (int x = -tileSplash; x <= tileSplash; x++)
            {
                for (int y = -tileSplash; y <= tileSplash; y++)
                {
                    Vector2Int localGridPos = new Vector2Int(x, y);
                    float distance = Mathf.Sqrt(Mathf.Pow(localGridPos.x, 2f)
                                                + Mathf.Pow(localGridPos.y, 2f));
                    float maxDistance = tileSplash + .5f;
                    bool inCircle = distance <= maxDistance;
                    if (!inCircle) continue;

                    Vector2Int splashPos = gridPos + localGridPos;

                    if (Station.Singleton.TryGetFloorAtGridPos(splashPos, out FloorTile floor))
                    {
                        floor.TakeDamage(new DamageReport(50));
                        //splashStructure.health -= 100f * 1f; //Mathf.Pow(.5f, 4f * (distance  / maxDistance));
                    }
                }
            }
        }
    }
}