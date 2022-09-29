using System.Collections.Generic;
using UnityEngine;

namespace _Project.Codebase
{
    [CreateAssetMenu(fileName = "Construct Tile Data", menuName = "Construct Tiles/Data", order = 0)]
    public class PlaceableScriptable : ScriptableObject
    {
        public List<TileConstructPrefabData> tileConstructData = new List<TileConstructPrefabData>();
        public List<StructurePrefabData> structureData = new List<StructurePrefabData>();
    }
}