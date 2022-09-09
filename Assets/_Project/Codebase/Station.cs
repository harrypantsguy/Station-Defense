using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Debug = UnityEngine.Debug;

namespace _Project.Codebase
{
    public class Station : MonoSingleton<Station>
    {
        [SerializeField] private Tilemap _wallMap;
        [SerializeField] private Tilemap _floorMap;
        
        private Dictionary<Vector2Int, Structure> _structureGrid = new Dictionary<Vector2Int, Structure>();

        private const float TILEMAP_SCALE_MULTIPLIER = .25f;

        public Vector3Int WorldToGridPos(Vector2 pos) => _wallMap.WorldToCell(pos);
        private Vector2Int GetClosestTileToPos(Vector2 pos) => new Vector2Int(Mathf.CeilToInt(pos.x / TILEMAP_SCALE_MULTIPLIER), 
            Mathf.CeilToInt(pos.y / TILEMAP_SCALE_MULTIPLIER));
        public Vector2Int WorldToGridPos2D(Vector2 pos) => (Vector2Int)WorldToGridPos(pos);
        public Vector2 GridToWorldPos(Vector2Int gridPos) => gridPos + new Vector2(.5f, .5f);
        public Vector2 SnapPointToGrid(Vector2 pos) => GridToWorldPos(WorldToGridPos2D(pos));
        
        private bool TryGetStructureAtPos(Vector2 pos, out Structure structure) =>
            TryGetStructureAtGridPos(WorldToGridPos2D(pos), out structure);
        private bool TryGetStructureAtGridPos(Vector2Int pos, out Structure structure)
        {
            if (_structureGrid.TryGetValue(pos, out structure))
                return true;
            return false;
        }

        public bool IsStructureAtGridPos(Vector2Int pos) => TryGetStructureAtGridPos(pos, out Structure s);
        public bool IsStructureAtWorldPos(Vector2 pos) => TryGetStructureAtPos(pos, out Structure s);

        public bool IsValidPlacementAtWorldPos(Vector2 pos) => IsValidPlacementAtGridPos(WorldToGridPos2D(pos));
        public bool IsValidPlacementAtGridPos(Vector2Int pos)
        {
            if (pos.x < 2 && pos.x > -3 && pos.y > -3 && pos.y < 2)
                return false;

            if (IsStructureAtGridPos(pos))
                return false;

            if (!HasNecessaryNeighborsAtPos(pos))
                return false;

            return true;
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
        private bool HasNecessaryNeighborsAtPos(Vector2Int gridPos)
        {
            return IsStructureAtGridPos(gridPos + new Vector2Int(-1, 0)) ||
                   IsStructureAtGridPos(gridPos + new Vector2Int(0, 1)) ||
                   IsStructureAtGridPos(gridPos + new Vector2Int(1, 0)) ||
                   IsStructureAtGridPos(gridPos + new Vector2Int(0, -1)) || IsGridPosAdjacentToPowerCore(gridPos);
        }

        public bool IsRectViableToFill(Vector2 corner1, Vector2 corner2, Structure structure) =>
            IsRectViableToFill(WorldToGridPos2D(corner1), WorldToGridPos2D(corner2), structure);
        public bool IsRectViableToFill(Vector2Int corner1, Vector2Int corner2, Structure structure,
            bool returnOnViabilityAssessment = true) => IsRectViableToFill(corner1, corner2, structure,
            returnOnViabilityAssessment, out List<Vector2Int> viablePoints);
        public bool IsRectViableToFill(Vector2Int corner1, Vector2Int corner2, Structure structure, 
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

            bool structureIsNull = structure == null;
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

                    if (!structureIsNull && (atSmallestX || atBiggestX) && (atSmallestY || atBiggestY)) continue;
                    
                    bool isStructureAtGridPos = false;
                    bool checkedGridPosForStructure = false;
                    bool outsideRect = pos.x > maxX || pos.x < minX || pos.y < minY || pos.y > maxY;

                    if (structureIsNull)
                        hasFoundValidPos = true;

                    if (!hasFoundValidPos)
                    {
                        isStructureAtGridPos = IsStructureAtGridPos(pos);
                        checkedGridPosForStructure = true;
                        if (isStructureAtGridPos || !outsideRect && IsGridPosAdjacentToPowerCore(pos))
                            hasFoundValidPos = true;
                    }

                    if (hasFoundValidPos && returnOnViabilityAssessment)
                        return true;

                    if (outsideRect || IsInsidePowerCore(pos)) continue;
                    
                    if (!structureIsNull && !checkedGridPosForStructure)
                        isStructureAtGridPos = IsStructureAtGridPos(pos);
                    
                    if (structureIsNull || !isStructureAtGridPos)
                        viablePoints.Add(pos);
                }
            }

            return hasFoundValidPos;
        }
        public void FillRectWithStructure(Vector2 corner1, Vector2 corner2, Structure structure) => 
            FillRectWithStructure(WorldToGridPos2D(corner1), WorldToGridPos2D(corner2), structure);
        public void FillRectWithStructure(Vector2Int corner1, Vector2Int corner2, Structure structure)
        {
            if (IsRectViableToFill(corner1, corner2, structure, false, out List<Vector2Int> positionsToSet))
                foreach (Vector2Int pos in positionsToSet)
                    SetTileToStructure(pos, structure == null ? null : new Structure(structure));
        }
        public bool TrySetGridPosStructure(Vector2 position, Structure structure) => 
            TrySetGridPosStructure(WorldToGridPos2D(position), structure);

        public bool TrySetGridPosStructure(Vector2Int gridPos, Structure structure)
        {
            if (structure == null || IsValidPlacementAtGridPos(gridPos))
            {
                SetTileToStructure(gridPos, structure);
                return true;
            }

            return false;
        }

        public void RemoveStructure(Vector2Int tilePos)
        {
            SetTileToStructure(tilePos, null);
        }
        
        public void RemoveStructure(Vector2 position)
        {
            RemoveStructure(WorldToGridPos2D(position));
        }

        private void SetTileToStructure(Vector2Int gridPos, Structure structure)
        {
            _structureGrid.Remove(gridPos);
            
            bool structureIsNull = structure == null;
            
            if (!structureIsNull)
            {
                structure.gridPos = gridPos;

                _structureGrid.Add(gridPos, structure);
            }

            _wallMap.SetTile((Vector3Int)gridPos, structureIsNull ? null : References.Singleton.structureTileDictionary[structure.structureName]);
        }

        public bool TakeDamage(Vector2 location, int tileSplash)
        {
           // Debug.Log("hit");
            if (!TryGetStructureAtPos(location, out Structure hitStructure))
            {
                //Debug.Break();
                //Debug.LogError("failed to locate structure at hit point");
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
                    
                    Vector2Int gridPos = hitStructure.gridPos + localGridPos;

                    if (!TryGetStructureAtGridPos(gridPos, out Structure splashStructure))
                        continue;
                    
                    splashStructure.health -= 100f * 1f; //Mathf.Pow(.5f, 4f * (distance  / maxDistance));
                    if (splashStructure.health <= 0f)
                        RemoveStructure(gridPos);
                }
            }

            return true;
        }
    }
}