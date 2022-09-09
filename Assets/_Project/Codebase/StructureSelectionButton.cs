using UnityEngine;

namespace _Project.Codebase
{
    public class StructureSelectionButton : CustomUI
    {
        [SerializeField] private StructureName structureName;
        public void SetStructure()
        {
            Player.Singleton.SetStructure(structureName);
        }
    }
}