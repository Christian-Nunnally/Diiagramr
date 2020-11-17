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
        /// <summary>
        /// A list of strings representing the top level menu options on the tool bar.
        /// </summary>
        public abstract ObservableCollection<string> TopLevelMenuNames { get; }

        /// <summary>
        /// Occurs when the user clicks on a top level menu item.
        /// </summary>
        /// <param name="sender">The top level menu item visual that was clicked on.</param>
        /// <param name="e">The event arguments.</param>
        public abstract void OpenContextMenuForTopLevelMenuHandler(object sender, MouseEventArgs e);

        /// <summary>
        /// Opens one of the top level tool bar menu items.
        /// </summary>
        /// <param name="position">The position to open the context menu at.</param>
        /// <param name="topLevelMenuName">The name of the menu item to open.</param>
        public abstract void OpenContextMenuForTopLevelMenu(Point position, string topLevelMenuName);
    }
}