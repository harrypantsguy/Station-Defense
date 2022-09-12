using System.Collections.Generic;
using System.Diagnostics;
using DanonsTools.Plugins.DanonsTools.Utilities;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Debug = UnityEngine.Debug;

namespace _Project.Codebase
{
    public class FloorTile : TileConstruct
    {
        public IPlaceable Placeable { get; private set; }
        private WallTile _wallTile;
        
        public FloorTile(PlaceableName placeableName, Vector2Int gridPos, bool blockDeletion = false) :
            base(placeableName, ConstructType.Floor, gridPos, blockDeletion)
        {
        }

        public FloorTile(PlaceableName placeableName, bool blockDeletion = false) : 
            this(placeableName, Vector2Int.zero, blockDeletion)
        {
        }

        public FloorTile(TileConstruct construct) : base(construct)
        {
            FloorTile original = construct as FloorTile;
            if (original == null) Debug.LogError("Improper Tile Construct copy");
            Placeable = original.Placeable;
            // you need to place a copy of the placeable on the station!
        }

        public override void Update()
        {
            base.Update();
            _wallTile?.Update();
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

        public override bool IsValidRectPlacement(Station station, in Vector2Int corner1, in Vector2Int corner2,
            bool borderOnly, bool returnOnValidityAssessment,
            out List<Vector2Int> validPositions)
        {
            validPositions = new List<Vector2Int>();
            Utils.GetMinAndMax(corner1, corner2, out int minX, out int minY, out int maxX, out int maxY);

            int extendedMinX = minX - 1;
            int extendedMinY = minY - 1;
            int extendedMaxX = maxX + 1;
            int extendedMaxY = maxY + 1;

            int shrunkenMinX = minX + 1, shrunkenMinY = minY + 1, shrunkenMaxX = maxX - 1, shrunkenMaxY = maxY - 1;

            bool rectIsDeterminedValid = false;
            bool hasFoundOccupiedPos = false;
            bool hasFoundUnoccupiedPosInsideRect = false;
            List<Vector2Int> positionsToCheck = Utils.GenerateRectList(extendedMinX, extendedMinY,
                extendedMaxX, extendedMaxY, borderOnly);
            positionsToCheck.AddRange(Utils.GenerateRectList(minX, minY, maxX, maxY, true));

            bool checkingInsideRect = borderOnly && shrunkenMaxX >= shrunkenMinX && shrunkenMaxY >= shrunkenMinY;
            
            if (checkingInsideRect)
            {
                positionsToCheck.AddRange(Utils.GenerateRectList(shrunkenMinX, shrunkenMinY, 
                    shrunkenMaxX, shrunkenMaxY, true));
            }
            Debug.Log(checkingInsideRect);
            
            foreach (Vector2Int pos in positionsToCheck)
            {
                bool atSmallestX = pos.x == extendedMinX;
                bool atBiggestX = pos.x == extendedMaxX;
                bool atSmallestY = pos.y == extendedMinY;
                bool atBiggestY = pos.y == extendedMaxY;

                if ((atSmallestX || atBiggestX) && (atSmallestY || atBiggestY)) continue;

                bool isGridPosOccupied = false;
                bool checkedPosForValidity = false;
                bool outsideTrueRect = atSmallestX || atSmallestY || atBiggestX || atBiggestY || 
                                       checkingInsideRect && (pos.x >= shrunkenMinX && pos.x <= shrunkenMaxX) && 
                                        pos.y <= shrunkenMaxY && pos.y >= shrunkenMinY;

                if (!rectIsDeterminedValid)
                {
                    isGridPosOccupied = station.IsFloorAtGridPos(pos);
                    checkedPosForValidity = true;

                    if (isGridPosOccupied)
                        hasFoundOccupiedPos = true;
                    else if (!outsideTrueRect)
                        hasFoundUnoccupiedPosInsideRect = true;

                    if (hasFoundOccupiedPos && hasFoundUnoccupiedPosInsideRect)
                    {
                        rectIsDeterminedValid = true;
                        if (returnOnValidityAssessment) return true;
                    }
                }

                if (outsideTrueRect) continue;

                if (!checkedPosForValidity)
                    isGridPosOccupied = station.IsFloorAtGridPos(pos);

                if (!isGridPosOccupied)
                    validPositions.Add(pos);

            }
            return rectIsDeterminedValid;
        }

        public override void Delete(Station station)
        {
            if (Placeable != null)
            {
                if (Placeable.BlockDeletion) return;
                Placeable.Delete(station);
            }

            if (BlockDeletion) return;
            
            station.RemoveFloorFromDictAndTilemapAtGridPos(gridPos);
        }

        public void SetPlaceable(Station station, IPlaceable placeable)
        {
            Placeable = placeable;
            if (placeable is WallTile wall)
            {
                _wallTile = wall;
                station.SetWallMapTile((Vector3Int) gridPos, References.Singleton.GetTile(placeable.PlaceableName));
            }
        }

        public void RemovePlaceable(Station station)
        {
            Placeable = null;
            _wallTile = null;
            station.SetWallMapTile((Vector3Int) gridPos, null);
        }
    }
}