using UnityEngine;

namespace _Project.Codebase
{
    public interface IPlaceable
    {
        public PlaceableName PlaceableName { get; set; }
        public bool IsValidPlacementAtGridPos(Station station, in Vector2Int gridPos);
        public void TryPlace(Station station, in Vector2Int gridPos, bool ignoreValidity = false);
        public void Delete(Station station);
    }
}