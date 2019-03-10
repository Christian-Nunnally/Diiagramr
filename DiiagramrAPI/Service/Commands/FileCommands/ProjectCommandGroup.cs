using DiiagramrAPI.Shell;
using DiiagramrAPI.ViewModel;
using System.Windows;

namespace DiiagramrAPI.Service.Commands.FileCommands
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
