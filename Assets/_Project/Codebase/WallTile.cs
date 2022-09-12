using System.Collections.Generic;
using FishingGame.Utilities;
using UnityEngine;

namespace _Project.Codebase
{
    public class WallTile : TileConstruct, IDestroyable
    {
        public float Health { get; set; } = 100f;
        public FloorTile floor;
        public void TakeDamage()
        {
        }
        public void Die()
        {
        }

        public override void Update()
        {
            base.Update();
        }

        public override bool IsValidPlacementAtGridPos(Station station, in Vector2Int gridPos) => 
            station.DoesGridPosHaveFloorButNoPlaceable(gridPos);

        private bool IsValidPlacementAtGridPos(Station station, in Vector2Int gridPos, out FloorTile foundFloor) => 
            station.DoesGridPosHaveFloorButNoPlaceable(gridPos, out foundFloor);
        
        public override void TryPlace(Station station, in Vector2Int gridPos, bool ignoreValidity)
        {
            if (!ignoreValidity && !IsValidPlacementAtGridPos(station, gridPos, out floor)) return;
            if (ignoreValidity && floor == null)
                station.TryGetFloorAtGridPos(gridPos, out floor);
            
            this.gridPos = gridPos;
            floor.SetPlaceable(station, this);
        }

        public override bool IsValidRectPlacement(Station station, in Vector2Int corner1, in Vector2Int corner2, bool returnOnValidityAssessment,
            out List<Vector2Int> validPositions)
        {
            validPositions = new List<Vector2Int>();
            bool hasFoundValidPos = false;
            foreach (Vector2Int pos in Utils.IterateOverRect(corner1, corner2))
            {
                if (station.DoesGridPosHaveFloorButNoPlaceable(pos))
                {
                    hasFoundValidPos = true;
                    if (returnOnValidityAssessment) return true;
                    validPositions.Add(pos);
                }
            }
            
            return hasFoundValidPos;
        }

        public override void Delete(Station station)
        {
            if (BlockDeletion) return;
            floor.RemovePlaceable(station);
        }

        public WallTile(PlaceableName placeableName, ConstructType type, Vector2Int gridPos, bool blockDeletion = false)
            : base(placeableName, type, gridPos, blockDeletion)
        {
        }

        public WallTile(PlaceableName placeableName, bool blockDeletion = false) : base(placeableName, blockDeletion)
        {
        }

        public WallTile(TileConstruct construct) : base(construct)
        {
            WallTile original = construct as WallTile;
            if (original == null) Debug.LogError("Improper Tile Construct copy");
            Health = original.Health;
            floor = original.floor;
        }
    }
}