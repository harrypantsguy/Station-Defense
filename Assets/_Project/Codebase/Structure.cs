using System.Collections.Generic;
using UnityEngine;

namespace _Project.Codebase
{
    public class Structure : MonoBehaviour, IDestroyable, IPlaceable
    {
        public PlaceableName PlaceableName { get; set; }
        public bool BlockDeletion { get; set; }
        public float BuildProgress { get; set; }
        public bool Built { get; set; }
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

        public bool IsValidRectPlacement(Station station, in Vector2Int corner1, in Vector2Int corner2,
            bool returnOnValidityAssessment, out List<Vector2Int> validPositions)
        {
            validPositions = new List<Vector2Int>();
            return false;
        }

        public void TryFillRect(Station station, in Vector2Int corner1, in Vector2Int corner2, bool ignoreValidity = false)
        {
        }

        public void Delete(Station station)
        {
        }
    }
}