namespace _Project.Codebase
{
    public class ToolSelectionButton : CustomUI
    {
        public ToolType type;
        public void SetPlacementType()
        {
            Player.Singleton.SetToolType(type);
        }
    }
}