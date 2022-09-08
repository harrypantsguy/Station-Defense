using System;

namespace _Project.Codebase 
{
    [Serializable]
    public class Structure
    {
        public float health;
        public StructureType type;

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