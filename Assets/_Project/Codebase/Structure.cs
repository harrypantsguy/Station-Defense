using System;
using UnityEngine;

namespace _Project.Codebase 
{
    [Serializable]
    public class Structure
    {
        public float health = 100f;
        public StructureName structureName;
        public StructureType type;
        public Vector2Int gridPos;

        public Structure(StructureName structureName)
        {
            this.structureName = structureName;
        }

        public Structure(Structure original)
        {
            if (original == null) return;
            
            health = original.health;
            structureName = original.structureName;
            gridPos = original.gridPos;
        }

        public static Structure GetStructureFromType(StructureName structureName)
        {
            switch (structureName)
            {
                case StructureName.None:
                    return null;
                default:
                    return new Structure(structureName);
            }
        }
    }
}