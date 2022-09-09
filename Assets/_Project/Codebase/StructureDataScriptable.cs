using System.Collections.Generic;
using UnityEngine;

namespace _Project.Codebase
{
    [CreateAssetMenu(fileName = "StructureData", menuName = "Structures/Data", order = 0)]
    public class StructureDataScriptable : ScriptableObject
    {
        public List<StructurePrefabData> structureData = new List<StructurePrefabData>();
    }
}