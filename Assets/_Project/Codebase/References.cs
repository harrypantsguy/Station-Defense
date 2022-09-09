using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Project.Codebase
{
    public class References : MonoSingleton<References>
    {
        [SerializeField] private StructureDataScriptable _structureDataScriptable;
        public Dictionary<StructureName, StructurePrefabData> structureData = new Dictionary<StructureName, StructurePrefabData>();
        public Dictionary<StructureName, StructureType> structureTypes = new Dictionary<StructureName, StructureType>();
        public Dictionary<StructureName, Tile> structureTileDictionary = new Dictionary<StructureName, Tile>();
        public Dictionary<StructureName, Sprite> structureSpriteDictionary = new Dictionary<StructureName, Sprite>();

        private void Start()
        {
            foreach (StructurePrefabData data in _structureDataScriptable.structureData)
                structureData.Add(data.structureName, data);
            foreach (StructurePrefabData data in  _structureDataScriptable.structureData)
                structureTypes.Add(data.structureName, data.type);
            foreach (StructurePrefabData data in  _structureDataScriptable.structureData)
                structureTileDictionary.Add(data.structureName, data.tile);
            foreach (StructurePrefabData data in  _structureDataScriptable.structureData)
                structureSpriteDictionary.Add(data.structureName, data.sprite);
        }
    }
}