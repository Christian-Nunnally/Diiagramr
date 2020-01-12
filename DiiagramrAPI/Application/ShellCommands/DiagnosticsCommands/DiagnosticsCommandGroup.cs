using System.Windows;

namespace DiiagramrAPI.Application.ShellCommands.DiagnosticsCommands
{
    public class DiagnosticsCommandGroup : TopLevelToolBarCommand
    {
        public override string Name => "Diagnostics";

        public override float Weight => 1.0f;

        internal override void ExecuteInternal(IApplicationShell shell, object parameter)
        {
            if (parameter is Point point)
            {
                shell.ShowContextMenu(SubCommandItems, new Point(point.X + 1, point.Y + 19));
            }
        }
    }
}