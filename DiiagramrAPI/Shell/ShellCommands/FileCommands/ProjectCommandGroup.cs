using DiiagramrAPI.Shell;
using System.Windows;

namespace DiiagramrAPI.Shell.ShellCommands.FileCommands
{
    public class ProjectCommandGroup : TopLevelToolBarCommand
    {
        public override string Name => "Project";

        internal override void ExecuteInternal(IShell shell, object parameter)
        {
            if (parameter is Point point)
            {
                shell.ShowContextMenu(SubCommandItems, new Point(point.X + 1, point.Y + 19));
            }
        }
    }
}
