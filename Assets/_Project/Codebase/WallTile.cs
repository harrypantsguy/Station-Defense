
using UnityEngine;

namespace _Project.Codebase
{
    public class WallTile : TileConstruct, IDestroyable
    {
        public float Health { get; set; }
        public void TakeDamage()
        {
        }
        public void Die()
        {
        }

        public override bool IsValidPlacementAtGridPos(Station station, in Vector2Int gridPos)
        {
            if (station.TryGetFloorAtGridPos(gridPos, out FloorTile floor))
            {
                return floor.Placeable == null;
            }

            return false;
        }

        public override void TryPlace(Station station, in Vector2Int gridPos, bool ignoreValidity)
        {
            if (!ignoreValidity && station.TryGetFloorAtGridPos(gridPos, out FloorTile floor))
            {
                if (floor.Placeable == null)
                {
                    this.gridPos = gridPos;
                    floor.SetPlaceable(station, this);
                }
            }
        }

        public override void Delete(Station station)
        {
            if (station.TryGetFloorAtGridPos(gridPos, out FloorTile floor))
            {
                floor.RemovePlaceable(station);
            }
        }

        public WallTile(PlaceableName placeableName, ConstructType type, Vector2Int gridPos, bool blockPlayerReplacement) : base(placeableName, type, gridPos, blockPlayerReplacement)
        {
        }

        public WallTile(PlaceableName placeableName) : base(placeableName)
        {
        }

        public WallTile(TileConstruct construct) : base(construct)
        {
        }
    }
}