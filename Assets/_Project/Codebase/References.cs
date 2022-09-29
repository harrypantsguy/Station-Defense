using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Project.Codebase
{
    public class References : MonoSingleton<References>
    {
        [SerializeField] private PlaceableScriptable placeableScriptable;
        private Dictionary<PlaceableName, TileConstructPrefabData> _constructData = new Dictionary<PlaceableName, TileConstructPrefabData>();
        private Dictionary<PlaceableName, StructurePrefabData> _structureData = new Dictionary<PlaceableName, StructurePrefabData>();

        protected override void Awake()
        {
            foreach (TileConstructPrefabData data in placeableScriptable.tileConstructData)
            {
                if (!_constructData.TryAdd(data.placeableName, data))
                {
                    Debug.LogError("Failure to create construct dictionary, check TileConstruct Data scriptable");
                }
            }
            
            foreach (StructurePrefabData data in placeableScriptable.structureData)
            {
                if (!_structureData.TryAdd(data.placeableName, data))
                {
                    Debug.LogError("Failure to create construct dictionary, check Structure Data scriptable");
                }
            }

            base.Awake();
        }

        public PlaceableType GetType(PlaceableName name)
        {
            if (_constructData.TryGetValue(name, out TileConstructPrefabData data))
                return data.type;
            return PlaceableType.Structure;
        }

        public Sprite GetSprite(PlaceableName name)
        {
            if (_constructData.TryGetValue(name, out TileConstructPrefabData data))
                return data.sprite;
            return _structureData[name].sprite;
        }
        public Tile GetTile(PlaceableName name) => _constructData[name].tile;

        public GameObject GetStructure(PlaceableName name) => _structureData[name].structurePrefab;

        public ResourcesContainer GetCost(PlaceableName name)
        {
            if (_constructData.TryGetValue(name, out TileConstructPrefabData data))
                return data.placementCost;
            return _structureData[name].placementCost;
        }

        public StructurePrefabData GetStructurePrefabData(PlaceableName name) => _structureData[name];
    }
}