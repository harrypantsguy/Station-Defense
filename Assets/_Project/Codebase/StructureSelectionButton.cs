using UnityEngine;

namespace _Project.Codebase
{
    public class StructureSelectionButton : CustomUI
    {
        [SerializeField] private PlaceableName placeableName;
        public void SetStructure()
        {
            Player.Singleton.SetStructure(placeableName);
        }
    }
}