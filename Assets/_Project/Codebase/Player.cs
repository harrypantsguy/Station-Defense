using FishingGame.Utilities;

namespace _Project.Codebase
{
    public class Player : MonoSingleton<Player>
    {
        public StructureType structureType;

        public void SetStructure(StructureType type)
        {
            structureType = type;
        }

        private void Update()
        {
            if (CustomUI.MouseOverUI) return;
            
            if (GameControls.PlaceStructure.IsHeld)
            {
                Station.Singleton.PlaceStructure(Utils.WorldMousePos, Structure.GetStructureFromType(structureType));
            }
            else if (GameControls.RemoveStructure.IsHeld)
            {
                Station.Singleton.RemoveStructure(Utils.WorldMousePos);
            }
        }
    }
}
