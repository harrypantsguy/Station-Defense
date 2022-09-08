using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Project.Codebase
{
    public class Station : MonoSingleton<Station>
    {
        [SerializeField] private List<StructureTileData> _tiles;
        [SerializeField] private Tilemap _wallMap;
        [SerializeField] private Tilemap _floorMap;

        private Dictionary<Vector2Int, Structure> _structureGrid = new Dictionary<Vector2Int, Structure>();
        private Dictionary<StructureType, Tile> _tileDictionary = new Dictionary<StructureType, Tile>();

        private void Start()
        {
            foreach (StructureTileData data in _tiles)
                _tileDictionary.Add(data.type, data.tile);
        }

        private Vector3Int WorldToTilePos(Vector2 pos) => _wallMap.WorldToCell(pos);
        private Vector2Int WorldToTilePos2D(Vector2 pos) => (Vector2Int)WorldToTilePos(pos);

        public void RemoveStructure(Vector2 position)
        {
            Vector3Int tilePos = WorldToTilePos(position);
            if (!_structureGrid.TryGetValue((Vector2Int) tilePos, out Structure s))
                return;
            
            _structureGrid[(Vector2Int)tilePos] = null;
            _wallMap.SetTile(tilePos, null);
        }
        
        public void PlaceStructure(Vector2 position, Structure structure)
        {
            Vector3Int tilePos = WorldToTilePos(position);
            if (_structureGrid.TryGetValue((Vector2Int) tilePos, out Structure s))
            {
                _structureGrid[(Vector2Int)tilePos] = null;
            }
            
            _structureGrid[(Vector2Int)tilePos] = structure;
            _wallMap.SetTile(tilePos, _tileDictionary[structure.type]);
        }
    }
}