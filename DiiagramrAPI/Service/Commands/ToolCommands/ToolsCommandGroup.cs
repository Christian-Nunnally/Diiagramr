using DiiagramrAPI.Shell;
using DiiagramrAPI.ViewModel;
using System.Windows;

namespace DiiagramrAPI.Service.Commands.ToolCommands
{
    public class ToolsCommandGroup : TopLevelToolBarCommand
    {
        public override string Name => "Tools";
        public override float Weight => 0.5f;

        internal override void ExecuteInternal(IShell shell, object parameter)
        {
            if (parameter is Point point)
            {
                shell.ShowContextMenu(SubCommandItems, new Point(point.X, point.Y + 19));
            }
        }
    }
}
