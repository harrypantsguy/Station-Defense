using System.Collections.Generic;
using UnityEngine;

namespace _Project.Codebase
{
    [CreateAssetMenu(fileName = "Construct Tile Data", menuName = "Construct Tiles/Data", order = 0)]
    public class ConstructTileDataScriptable : ScriptableObject
    {
        public List<TileConstructPrefabData> structureData = new List<TileConstructPrefabData>();
    }
}