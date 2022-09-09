using System;
using UnityEngine;

namespace _Project.Codebase 
{
    [Serializable]
    public class Structure
    {
        public float health = 50f;
        public StructureType type;
        public Vector2Int gridPos;

        public static Structure GetStructureFromType(StructureType type)
        {
            switch (type)
            {
                default:
                    return new Structure();
            }
        }
    }
}