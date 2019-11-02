using System.Windows;

namespace DiiagramrAPI.Application.ShellCommands.HelpCommands
{
    public class HelpCommandGroup : TopLevelToolBarCommand
    {
        public override string Name => "Help";
        public override float Weight => 0.6f;

        internal override void ExecuteInternal(IApplicationShell shell, object parameter)
        {
            if (parameter is Point point)
            {
                shell.ShowContextMenu(SubCommandItems, new Point(point.X, point.Y + 19));
            }
        }
    }
}