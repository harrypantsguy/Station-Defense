using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Debug = UnityEngine.Debug;

namespace _Project.Codebase
{
    public class Station : MonoSingleton<Station>
    {
        [SerializeField] private List<StructureTileData> _tiles;
        [SerializeField] private Tilemap _wallMap;
        [SerializeField] private Tilemap _floorMap;

        public CompositeCollider2D compositeCollider;
        
        private Dictionary<Vector2Int, Structure> _structureGrid = new Dictionary<Vector2Int, Structure>();
        private Dictionary<StructureType, Tile> _tileDictionary = new Dictionary<StructureType, Tile>();

        private const float TILEMAP_SCALE_MULTIPLIER = .25f;

        private void Start()
        {
            foreach (StructureTileData data in _tiles)
                _tileDictionary.Add(data.type, data.tile);

            compositeCollider = _wallMap.GetComponent<CompositeCollider2D>();
        }

        private Vector3Int WorldToTilePos(Vector2 pos) => _wallMap.WorldToCell(pos);
        private Vector2Int GetClosestTileToPos(Vector2 pos) => new Vector2Int(Mathf.CeilToInt(pos.x / TILEMAP_SCALE_MULTIPLIER), 
            Mathf.CeilToInt(pos.y / TILEMAP_SCALE_MULTIPLIER));
        private Vector2Int WorldToTilePos2D(Vector2 pos) => (Vector2Int)WorldToTilePos(pos);
        private bool TryGetStructureAtPos(Vector2 pos, out Structure structure) =>
            TryGetStructureAtGridPos(WorldToTilePos2D(pos), out structure);
        private bool TryGetStructureAtGridPos(Vector2Int pos, out Structure structure)
        {
            if (_structureGrid.TryGetValue(pos, out structure))
                return true;
            return false;
        }
        
        public void RemoveStructure(Vector2 position)
        {
            RemoveStructure(WorldToTilePos2D(position));
        }

        public void RemoveStructure(Vector2Int tilePos)
        {
            if (!TryGetStructureAtGridPos(tilePos, out Structure s))
                return;

            _structureGrid.Remove(tilePos);
            _wallMap.SetTile((Vector3Int)tilePos, null);
        }

        public void PlaceStructure(Vector2 position, Structure structure)
        {
            Vector2Int tilePos = WorldToTilePos2D(position);
            if (_structureGrid.TryGetValue(tilePos, out Structure s))
            {
                _structureGrid[tilePos] = null;
            }

            structure.gridPos = tilePos;
            
            _structureGrid[tilePos] = structure;
            _wallMap.SetTile((Vector3Int)tilePos, _tileDictionary[structure.type]);
        }

        public bool TakeDamage(Vector2 location, int tileSplash)
        {
            Debug.Log("hit");
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
                    Vector2Int gridPos = hitStructure.gridPos + new Vector2Int(x, y);
                    if (!TryGetStructureAtGridPos(gridPos, out Structure splashStructure))
                        continue;
                    
                    Debug.Log(splashStructure);
                    
                    splashStructure.health -= 100f;
                    if (splashStructure.health <= 0f)
                        RemoveStructure(gridPos);
                }
            }

            return true;
        }
    }
}