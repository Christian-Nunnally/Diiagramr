using System.Windows;

namespace DiiagramrAPI.Application.ShellCommands.ToolCommands
{
    public class ToolsCommandGroup : TopLevelToolBarCommand
    {
        public override string Name => "Tools";

        public override float Weight => 0.5f;

        internal override void ExecuteInternal(IApplicationShell shell, object parameter)
        {
            if (parameter is Point point)
            {
                shell.ShowContextMenu(SubCommandItems, new Point(point.X, point.Y + 19));
            }
        }
    }
}