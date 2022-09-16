using System.Collections.Generic;
using DanonsTools.Plugins.DanonsTools.Utilities;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Project.Codebase
{
    public class Station : MonoSingleton<Station>
    {
        [SerializeField] private Tilemap _wallMap;
        [SerializeField] private Tilemap _floorMap;
        [SerializeField] private Structure _powerCore;
        
        private Dictionary<Vector2Int, FloorTile> _floorGrid = new Dictionary<Vector2Int, FloorTile>();

        private void Start()
        {
            _powerCore.BlockDeletion = true;
            FloorTile powerCoreFloorTile = new FloorTile(PlaceableName.GratedFloor, true);
            powerCoreFloorTile.TryFillRect(this,
                 new Vector2Int(-3, -3), new Vector2Int(2,2), true, true);
            powerCoreFloorTile.SetPlaceable(this, _powerCore);
            powerCoreFloorTile.TryFillRect(this,
                new Vector2Int(-2, -2), new Vector2Int(1,1), false, true);
        }

        private void Update()
        {
            foreach (FloorTile tile in _floorGrid.Values)
            {
                tile.Update();
            }
        }

        public Vector3Int WorldToGridPos(Vector2 pos) => _wallMap.WorldToCell(pos);
        public Vector2Int WorldToGridPos2D(Vector2 pos) => (Vector2Int)WorldToGridPos(pos);
        public Vector2 GridToWorldPos(Vector2Int gridPos) => gridPos + new Vector2(.5f, .5f);
        public Vector2 SnapPointToGrid(Vector2 pos) => GridToWorldPos(WorldToGridPos2D(pos));

        public bool TryGetFloorAtGridPos(Vector2Int gridPos, out FloorTile floor) =>
            _floorGrid.TryGetValue(gridPos, out floor); 

        public bool IsFloorAtGridPos(Vector2Int gridPos) => TryGetFloorAtGridPos(gridPos, out FloorTile floor);
        
        public void SetFloorAtGridPos(Vector2Int gridPos, FloorTile tile)
        {
            _floorGrid[gridPos] = tile;
            _floorMap.SetTile((Vector3Int)gridPos, References.Singleton.GetTile(tile.PlaceableName));
        }
        
        /// <summary>
        /// See's if floor is at grid pos, calls .Delete() on it.
        /// </summary>
        /// <param name="gridPos"></param>
        public void TryRemoveFloorAtGridPos(Vector2Int gridPos)
        {
            if (TryGetFloorAtGridPos(gridPos, out FloorTile floor))
            {
                floor.Delete(this);
            }
        }
        /// <summary>
        /// Doesn't call .Delete() on floor tile. Likely to be called from a TileConstruct. 
        /// </summary>
        /// <param name="gridPos"></param>
        public void RemoveFloorFromDictAndTilemapAtGridPos(Vector2Int gridPos)
        {
            _floorMap.SetTile((Vector3Int)gridPos, null);
            _floorGrid.Remove(gridPos);
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

        public bool DoesGridPosHaveFloorButNoPlaceable(Vector2Int gridPos) =>
            DoesGridPosHaveFloorButNoPlaceable(gridPos, out FloorTile floor);
        public bool DoesGridPosHaveFloorButNoPlaceable(Vector2Int gridPos, out FloorTile floor)
        {
            if (TryGetFloorAtGridPos(gridPos, out floor))
            {
                return floor.Placeable == null;
            }

            return false;
        }
        
        public bool IsPlaceableAtGridPos(Vector2Int gridPos) =>
            TryGetPlaceableAtGridPos(gridPos, out IPlaceable placeable);

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

        public void FillPositionsWithConstruct(List<Vector2Int> positions, TileConstruct construct)
        {
            foreach (Vector2Int pos in positions)
            {
                IPlaceable newTile = IPlaceable.MakeCopy(construct);
                newTile.TryPlace(this, pos, true);
            }
        }

        public void RemoveAllOfTypeInRect(in Vector2Int corner1, in Vector2Int corner2, bool borderOnly, PlaceableType type)
        {
            foreach (Vector2Int pos in Utils.IterateOverRect(corner1, corner2, borderOnly))
            {
                if (type == PlaceableType.Floor)
                {
                    if (TryGetFloorAtGridPos(pos, out FloorTile floor))
                        floor.Delete(this);
                }
                else
                {
                    if (TryGetPlaceableAtGridPos(pos, out IPlaceable placeable))
                        placeable.Delete(this);
                }
            }
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