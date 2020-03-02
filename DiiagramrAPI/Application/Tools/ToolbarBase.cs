using DiiagramrAPI.Application.ShellCommands;
using Stylet;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace DiiagramrAPI.Application.Tools
{
    /// <summary>
    /// The toolbar of the shell. Contains a list of top level commands, each of which can contain child commands. All commands have weights to determine order.
    /// </summary>
    public abstract class ToolbarBase : Screen
    {
        public abstract ObservableCollection<IToolbarCommand> TopLevelMenuItems { get; }

        public abstract void ExecuteCommandHandler(object sender, MouseEventArgs e);
    }
}