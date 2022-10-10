using System.Collections.Generic;
using DanonsTools.Plugins.DanonsTools.Utilities;
using UnityEngine;

namespace _Project.Codebase
{
    public class WallTile : TileConstruct, IDamageable
    {
        public int Health { get; set; } = 100;
        public FloorTile floor;

        public DamageReport TakeDamage(DamageReport damage)
        {
            int actualDamage = Mathf.Min(damage.damage, Health);
            Health -= actualDamage;

            if (Health == 0)
            {
                Die();
            }
            
            return new DamageReport(damage.damage - actualDamage);
        }

        public void Die()
        {
            Delete();
        }

        public override bool IsValidPlacementAtGridPos(Station station, in Vector2Int gridPos, bool considerCost,
            out ResourcesContainer cost, out PlacementFailCause failCause)
        {
            cost = PlacementCost;
            failCause = PlacementFailCause.None;

            if (!IPlaceable.PlayerCanAfford(considerCost, cost))
            {
                failCause = PlacementFailCause.InsufficientFunds;
                return false;
            }

            if (!station.DoesGridPosHaveFloorButNoPlaceable(gridPos))
            {
                failCause = PlacementFailCause.ImproperLocation;
                return false;
            }

            return true;
        }

        public override void TryPlace(Station station, in Vector2Int gridPos, bool costResources, bool ignoreValidity)
        {
            if (!ignoreValidity && !IsValidPlacementAtGridPos(station, gridPos, costResources, 
                out ResourcesContainer cost, out PlacementFailCause failCause)) return;
            
            station.TryGetFloorAtGridPos(gridPos, out floor);
            
            this.gridPos = gridPos;
            floor.SetPlaceable(station, this);
            if (costResources)
                Player.Singleton.resources -= PlacementCost;
            Station = station;
        }

        public override bool IsValidRectPlacement(Station station, in Vector2Int corner1, in Vector2Int corner2, bool borderOnly, 
            bool returnOnValidityAssessment, out List<Vector2Int> validPositions, bool considerCost, 
            out ResourcesContainer cost, out PlacementFailCause failCause)
        {
            validPositions = new List<Vector2Int>();
            cost = new ResourcesContainer();
            failCause = PlacementFailCause.None;
            bool hasFoundValidPos = false;
            bool canAfford = true;
            foreach (Vector2Int pos in Utils.IterateOverRect(corner1, corner2, borderOnly))
            {
                if (station.DoesGridPosHaveFloorButNoPlaceable(pos))
                {
                    hasFoundValidPos = true;
                    cost += PlacementCost;
                    if (!IPlaceable.PlayerCanAfford(considerCost, cost))
                    {
                        canAfford = false;
                        failCause = PlacementFailCause.InsufficientFunds;
                    }

                    if (returnOnValidityAssessment) return canAfford;
                    validPositions.Add(pos);
                }
            }

            if (!hasFoundValidPos)
                failCause = PlacementFailCause.ImproperLocation;

            return hasFoundValidPos && canAfford;
        }

        public override void Delete()
        {
            if (BlockDeletion) return;
            floor.RemovePlaceable();
        }

        public WallTile(PlaceableName placeableName, PlaceableType type, Vector2Int gridPos, bool blockDeletion = false)
            : base(placeableName, type, gridPos, blockDeletion)
        {
        }

        public WallTile(PlaceableName placeableName, bool blockDeletion = false) : base(placeableName, blockDeletion)
        {
        }

        public WallTile(WallTile construct) : base(construct)
        {
            WallTile original = construct;
            if (original == null)
            {
                Debug.LogError("Improper Tile Construct copy");
                return;
            }
            Health = original.Health;
            floor = original.floor;
        }
    }
}