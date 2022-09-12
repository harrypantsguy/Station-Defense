using System.Collections.Generic;
using UnityEngine;

namespace _Project.Codebase
{
    public interface IPlaceable
    {
        public PlaceableName PlaceableName { get; set; }
        public bool BlockDeletion { get; set; }
        public float BuildProgress { get; set; }
        public bool Built { get; }
        public bool IsValidPlacementAtGridPos(Station station, in Vector2Int gridPos);
        public void TryPlace(Station station, in Vector2Int gridPos, bool ignoreValidity = false);
        public bool IsValidRectPlacement(Station station, in Vector2Int corner1, in Vector2Int corner2, bool borderOnly,
            bool returnOnValidityAssessment, out List<Vector2Int> validPositions);
        public void TryFillRect(Station station, in Vector2Int corner1, in Vector2Int corner2, bool borderOnly,
            bool ignoreValidity = false);
        public void Delete(Station station);
    }
}