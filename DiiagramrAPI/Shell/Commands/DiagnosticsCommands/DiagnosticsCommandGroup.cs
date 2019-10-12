using DiiagramrAPI.Shell;
using System.Windows;

namespace DiiagramrAPI.Service.Commands.DiagnosticsCommands
{
    public class DiagnosticsCommandGroup : TopLevelToolBarCommand
    {
        public override string Name => "Diagnostics";

        public override float Weight => 1.0f;

        internal override void ExecuteInternal(IShell shell, object parameter)
        {
            if (parameter is Point point)
            {
                shell.ShowContextMenu(SubCommandItems, new Point(point.X + 1, point.Y + 19));
            }
        }
    }
}
