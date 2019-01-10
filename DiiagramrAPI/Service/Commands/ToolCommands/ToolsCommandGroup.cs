using DiiagramrAPI.ViewModel;

namespace DiiagramrAPI.Service.Commands.ToolCommands
{
    public class ToolsCommandGroup : DiiagramrCommand
    {
        public override string Name => "Tools";
        public override float Weight => 0.5f;

        public override void Execute(ShellViewModel shell)
        {
            shell.ShowContextMenu(SubCommandItems);
        }
    }
}
