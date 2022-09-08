using UnityEngine;

namespace _Project.Codebase
{
    public class StructureSelectionButton : CustomUI
    {
        [SerializeField] private StructureType type;
        public void SetStructure()
        {
            Player.Singleton.SetStructure(type);
        }
    }
}