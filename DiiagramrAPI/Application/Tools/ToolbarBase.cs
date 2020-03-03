using Stylet;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace DiiagramrAPI.Application.Tools
{
    /// <summary>
    /// The toolbar of the shell. Contains a list of top level commands, each of which can contain child commands. All commands have weights to determine order.
    /// </summary>
    public abstract class ToolbarBase : Screen
    {
        public abstract ObservableCollection<string> TopLevelMenuNames { get; }

        public abstract void OpenContextMenuForTopLevelMenuHandler(object sender, MouseEventArgs e);

        public abstract void OpenContextMenuForTopLevelMenu(Point position, string topLevelMenuName);
    }
}