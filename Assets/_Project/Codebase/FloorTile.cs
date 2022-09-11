using UnityEngine;

namespace _Project.Codebase
{
    public class FloorTile : TileConstruct
    {
        public IPlaceable Placeable { get; private set; }
        
        public FloorTile(PlaceableName placeableName, Vector2Int gridPos, bool blockPlayerReplacement) :
            base(placeableName, ConstructType.Floor, gridPos, blockPlayerReplacement)
        {
        }

        public FloorTile(PlaceableName placeableName, bool blockPlayerReplacement = false) : 
            this(placeableName, Vector2Int.zero, blockPlayerReplacement)
        {
        }

        public FloorTile(TileConstruct construct) : base(construct)
        {
        }

        public override bool IsValidPlacementAtGridPos(Station station, in Vector2Int gridPos)
        {
            return !station.IsFloorAtGridPos(gridPos) && station.HasNeighborsAtPos(gridPos);
        }

        public override void TryPlace(Station station, in Vector2Int gridPos, bool ignoreValidity)
        {
            if (!ignoreValidity && !IsValidPlacementAtGridPos(station, gridPos)) return;
            
            this.gridPos = gridPos;
            station.SetFloorAtGridPos(gridPos, this);
        }

        public override void Delete(Station station)
        {
            RemovePlaceable(station);
        }

        public void SetPlaceable(Station station, IPlaceable placeable)
        {
            Placeable = placeable;
            station.SetWallMapTile((Vector3Int)gridPos, References.Singleton.GetTile(placeable.PlaceableName));
        }

        public void RemovePlaceable(Station station)
        {
            Placeable = null;
            station.SetWallMapTile((Vector3Int) gridPos, null);
        }
    }
}