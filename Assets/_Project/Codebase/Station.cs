using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Project.Codebase
{
    public class Station : MonoSingleton<Station>
    {
        [SerializeField] private Tilemap _wallMap;
        [SerializeField] private Tilemap _floorMap;
        
        private Dictionary<Vector2Int, FloorTile> _structureGrid = new Dictionary<Vector2Int, FloorTile>();

        private void Start()
        {
             TileConstruct powerCoreFloorTile = new FloorTile(PlaceableName.GratedFloor, true);
             powerCoreFloorTile.TryPlace(this, new Vector2Int(5, 5), true);
             
            //FillRectWithStructure(new Vector2Int(-2, -2), new Vector2Int(1,1), 
            //    powerCoreFloorTile, true);
        }
        public Vector3Int WorldToGridPos(Vector2 pos) => _wallMap.WorldToCell(pos);
        public Vector2Int WorldToGridPos2D(Vector2 pos) => (Vector2Int)WorldToGridPos(pos);
        public Vector2 GridToWorldPos(Vector2Int gridPos) => gridPos + new Vector2(.5f, .5f);
        public Vector2 SnapPointToGrid(Vector2 pos) => GridToWorldPos(WorldToGridPos2D(pos));

        public bool TryGetFloorAtGridPos(Vector2Int gridPos, out FloorTile floor) =>
            _structureGrid.TryGetValue(gridPos, out floor);

        public bool IsFloorAtGridPos(Vector2Int gridPos) => TryGetFloorAtGridPos(gridPos, out FloorTile floor);
        
        public void SetFloorAtGridPos(Vector2Int gridPos, FloorTile tile)
        {
            _structureGrid[gridPos] = tile;
            _floorMap.SetTile((Vector3Int)gridPos, References.Singleton.GetTile(tile.PlaceableName));
        }

        public void TryRemoveFloorAtGridPos(Vector2Int gridPos)
        {
            if (TryGetFloorAtGridPos(gridPos, out FloorTile floor))
            {
                _floorMap.SetTile((Vector3Int)gridPos, null);
                floor.Delete(this);
                _structureGrid.Remove(gridPos);
            }
        }

        public bool TryGetPlaceableAtGridPos(Vector2Int gridPos, out IPlaceable placeable)
        {
            placeable = null;
            if (TryGetFloorAtGridPos(gridPos, out FloorTile floor))
            {
                if (floor.Placeable != null)
                {
                    placeable = floor.Placeable;
                    return true;
                }
            }

            return false;
        }

        public void SetWallMapTile(Vector3Int gridPos, TileBase tile)
        {
            _wallMap.SetTile(gridPos, tile);
        }
        
        private bool IsInsidePowerCore(Vector2Int gridPos) =>
            IsInsidePowerCoreColumns(gridPos) && IsInsidePowerCoreRows(gridPos);
        private bool IsInsidePowerCoreColumns(Vector2Int gridPos) => gridPos.x > -3 && gridPos.x < 2;
        private bool IsInsidePowerCoreRows(Vector2Int gridPos) => gridPos.y < 2 && gridPos.y > -3;

        private bool IsGridPosAdjacentToPowerCore(Vector2Int gridPos)
        {
            bool inRowsInsideCore = IsInsidePowerCoreRows(gridPos);
            bool inColumnsInsideCore = IsInsidePowerCoreColumns(gridPos);

            return (gridPos.x == -3 || gridPos.x == 2) && inRowsInsideCore ||
                                       (gridPos.y == 2 || gridPos.y == -3) && inColumnsInsideCore;
        }
        public bool HasNeighborsAtPos(Vector2Int gridPos)
        {
            return IsFloorAtGridPos(gridPos + new Vector2Int(-1, 0)) ||
                   IsFloorAtGridPos(gridPos + new Vector2Int(0, 1)) ||
                   IsFloorAtGridPos(gridPos + new Vector2Int(1, 0)) ||
                   IsFloorAtGridPos(gridPos + new Vector2Int(0, -1));
        }

        public bool IsRectViableToFill(Vector2 corner1, Vector2 corner2, TileConstruct tileConstruct) =>
            IsRectViableToFill(WorldToGridPos2D(corner1), WorldToGridPos2D(corner2), tileConstruct);
        public bool IsRectViableToFill(Vector2Int corner1, Vector2Int corner2, TileConstruct tileConstruct,
            bool returnOnViabilityAssessment = true) => IsRectViableToFill(corner1, corner2, tileConstruct,
            returnOnViabilityAssessment, out List<Vector2Int> viablePoints);
        public bool IsRectViableToFill(Vector2Int corner1, Vector2Int corner2, TileConstruct tileConstruct, 
            bool returnOnViabilityAssessment, out List<Vector2Int> viablePoints)
        {
            viablePoints = new List<Vector2Int>();
            int minX = Mathf.Min(corner1.x, corner2.x);
            int maxX = Mathf.Max(corner1.x, corner2.x);
            int minY = Mathf.Min(corner1.y, corner2.y);
            int maxY = Mathf.Max(corner1.y, corner2.y);

            int extendedMinX = minX - 1;
            int extendedMaxX = maxX + 1;
            int extendedMinY = minY - 1;
            int extendedMaxY = maxY + 1;

            bool constructIsNull = tileConstruct == null;
            bool hasFoundValidPos = false;
            for (int x = extendedMinX; x <= extendedMaxX; x++)
            {
                for (int y = extendedMinY; y <= extendedMaxY; y++)
                {
                    Vector2Int pos = new Vector2Int(x, y);

                    bool atSmallestX = pos.x == extendedMinX;
                    bool atBiggestX = pos.x == extendedMaxX;
                    bool atSmallestY = pos.y == extendedMinY;
                    bool atBiggestY = pos.y == extendedMaxY;

                    if (!constructIsNull && (atSmallestX || atBiggestX) && (atSmallestY || atBiggestY)) continue;
                    
                    bool isStructureAtGridPos = false;
                    bool checkedGridPosForStructure = false;
                    bool outsideRect = pos.x > maxX || pos.x < minX || pos.y < minY || pos.y > maxY;

                    if (constructIsNull)
                        hasFoundValidPos = true;

                    if (!hasFoundValidPos)
                    {
                       // isStructureAtGridPos = IsConstructAtGridPos(pos);
                        checkedGridPosForStructure = true;
                        if (isStructureAtGridPos)
                            hasFoundValidPos = true;
                    }

                    if (hasFoundValidPos && returnOnViabilityAssessment)
                        return true;

                    if (outsideRect) continue;
                    
                    //if (!constructIsNull && !checkedGridPosForStructure)
                     //   isStructureAtGridPos = IsConstructAtGridPos(pos);
                    
                    if (constructIsNull || !isStructureAtGridPos)
                        viablePoints.Add(pos);
                }
            }

            return hasFoundValidPos;
        }
        public void FillRectWithStructure(Vector2 corner1, Vector2 corner2, TileConstruct tileConstruct) => 
            FillRectWithStructure(WorldToGridPos2D(corner1), WorldToGridPos2D(corner2), tileConstruct);
        public void FillRectWithStructure(Vector2Int corner1, Vector2Int corner2, TileConstruct tileConstruct, bool ignoreViability = false)
        {
            List<Vector2Int> positionsToSet = new List<Vector2Int>();
            if (ignoreViability)
            {
                for (int x = Mathf.Min(corner1.x, corner2.x); x <= Mathf.Max(corner1.x, corner2.x); x++)
                {
                    for (int y = Mathf.Min(corner1.y, corner2.y); y <= Mathf.Max(corner1.y, corner2.y); y++)
                    {
                        Vector2Int pos = new Vector2Int(x, y);
                        //if (!IsConstructAtGridPos(pos))
                            positionsToSet.Add(pos);
                    }
                }
            }
            
            if (ignoreViability || IsRectViableToFill(corner1, corner2, tileConstruct, false, out positionsToSet))
            {
                foreach (Vector2Int pos in positionsToSet)
                {
                    tileConstruct.TryPlace(this, pos);
                    //SetTileConstructAtPos(pos, tileConstruct);
                }
            }
        }
        public bool TrySetGridPosStructure(Vector2 position, TileConstruct tileConstruct) => 
            TrySetGridPosStructure(WorldToGridPos2D(position), tileConstruct);

        public bool TrySetGridPosStructure(Vector2Int gridPos, TileConstruct tileConstruct)
        {
            //if (tileConstruct == null || IsValidPlacementAtGridPos(gridPos, tileConstruct))
            {
               // SetTileConstructAtPos(gridPos, tileConstruct);
                return true;
            }

            return false;
        }

        public bool TakeDamage(Vector2 location, int tileSplash)
        {
            return true;
           // Debug.Log("hit");
            //if (!TryGetStructureAtPos(location, out TileConstruct hitStructure))
            {
                //Debug.Break();
                //Debug.LogError("failed to locate ConstructTile at hit point");
                Debug.DrawLine(Vector2.zero, location, Color.red);
                return false;
            }

            for (int x = -tileSplash; x <= tileSplash; x++)
            {
                for (int y = -tileSplash; y <= tileSplash; y++)
                {
                    Vector2Int localGridPos = new Vector2Int(x, y);
                    float distance = Mathf.Sqrt(Mathf.Pow(localGridPos.x, 2f)
                                                + Mathf.Pow(localGridPos.y, 2f));
                    float maxDistance = tileSplash + .5f;
                    bool inCircle = distance <= maxDistance;
                    if (!inCircle) continue;
                    
                    //Vector2Int gridPos = hitStructure.gridPos + localGridPos;
                    
                    
                    //if (!TryGetConstructAtGridPos(gridPos, out TileConstruct splashStructure))
                        continue;
                    
                   // splashStructure.health -= 100f * 1f; //Mathf.Pow(.5f, 4f * (distance  / maxDistance));
                    //if (splashStructure.health <= 0f)
                    //    RemoveStructure(gridPos);
                    //    RemoveStructure(gridPos);
                }
            }

            return true;
        }
    }
}