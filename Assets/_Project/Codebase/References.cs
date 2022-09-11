using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Project.Codebase
{
    public class References : MonoSingleton<References>
    {
        [SerializeField] private ConstructTileDataScriptable _constructTileDataScriptable;
        private Dictionary<PlaceableName, TileConstructPrefabData> _constructData = new Dictionary<PlaceableName, TileConstructPrefabData>();

        protected override void Awake()
        {
            foreach (TileConstructPrefabData data in _constructTileDataScriptable.structureData)
            {
                if (!_constructData.TryAdd(data.placeableName, data))
                {
                    Debug.LogError("Failure to create construct dictionary, check Structure Data scriptable");
                }
            }

            base.Awake();
        }

        public ConstructType GetType(PlaceableName name) => _constructData[name].type;
        public Sprite GetSprite(PlaceableName name) => _constructData[name].sprite;
        public Tile GetTile(PlaceableName name) => _constructData[name].tile;
    }
}