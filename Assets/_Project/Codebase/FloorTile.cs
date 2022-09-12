using System.Collections.Generic;
using UnityEngine;

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

        public override bool IsValidRectPlacement(Station station, in Vector2Int corner1, in Vector2Int corner2, bool returnOnValidityAssessment,
            out List<Vector2Int> validPositions)
        {
            validPositions = new List<Vector2Int>();
            int minX = Mathf.Min(corner1.x, corner2.x);
            int maxX = Mathf.Max(corner1.x, corner2.x);
            int minY = Mathf.Min(corner1.y, corner2.y);
            int maxY = Mathf.Max(corner1.y, corner2.y);
            
            int extendedMinX = minX - 1;
            int extendedMaxX = maxX + 1;
            int extendedMinY = minY - 1;
            int extendedMaxY = maxY + 1;

            bool rectIsDeterminedValid = false;
            bool hasFoundOccupiedPos = false;
            bool hasFoundUnoccupiedPosInsideRect = false;
            for (int x = extendedMinX; x <= extendedMaxX; x++)
            {
                for (int y = extendedMinY; y <= extendedMaxY; y++)
                {
                    Vector2Int pos = new Vector2Int(x, y);

                    bool atSmallestX = pos.x == extendedMinX;
                    bool atBiggestX = pos.x == extendedMaxX;
                    bool atSmallestY = pos.y == extendedMinY;
                    bool atBiggestY = pos.y == extendedMaxY;

                    if ((atSmallestX || atBiggestX) && (atSmallestY || atBiggestY)) continue;
                    
                    bool isGridPosOccupied = false;
                    bool checkedPosForValidity = false;
                    bool outsideRect = atSmallestX || atBiggestX || atSmallestY || atBiggestY;
                    
                    if (!rectIsDeterminedValid)
                    {
                        isGridPosOccupied = station.IsFloorAtGridPos(pos);
                        checkedPosForValidity = true;

                        if (isGridPosOccupied)
                            hasFoundOccupiedPos = true;
                        else if (!outsideRect)
                            hasFoundUnoccupiedPosInsideRect = true;
                        
                        if (hasFoundOccupiedPos && hasFoundUnoccupiedPosInsideRect)
                        {
                            rectIsDeterminedValid = true;
                            if (returnOnValidityAssessment) return true;
                        }
                    }

                    if (outsideRect) continue;
                    
                    if (!checkedPosForValidity)
                        isGridPosOccupied = station.IsFloorAtGridPos(pos);
                    
                    if (!isGridPosOccupied)
                        validPositions.Add(pos);
                }
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
                _wallTile = wall;
            station.SetWallMapTile((Vector3Int)gridPos, References.Singleton.GetTile(placeable.PlaceableName));
        }

        public void RemovePlaceable(Station station)
        {
            Placeable = null;
            _wallTile = null;
            station.SetWallMapTile((Vector3Int) gridPos, null);
        }
    }
}