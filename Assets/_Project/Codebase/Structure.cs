using UnityEngine;

namespace _Project.Codebase
{
    public class Structure : MonoBehaviour, IDestroyable, IPlaceable
    {
        public PlaceableName PlaceableName { get; set; }
        public Vector2Int dimensions;
        public Vector2 pivot;
        public float Health { get; set; } = 100f;
        
        public void TakeDamage()
        {
        }

        public void Die()
        {
        }


        public bool IsValidPlacementAtGridPos(Station station, in Vector2Int gridPos)
        {
            return false;
        }

        public void TryPlace(Station station, in Vector2Int gridPos, bool ignoreValidity)
        {
        }

        public void Delete(Station station)
        {
        }
    }
}