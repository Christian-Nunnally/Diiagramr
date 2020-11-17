using DiiagramrAPI.Service.Application;
using Stylet;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace DiiagramrAPI.Application.Tools
{
    /// <summary>
    /// A context menu that behaves like most context menus.
    /// </summary>
    public abstract class ContextMenuBase : Screen
    {
        /// <summary>
        /// Gets the list of commands that are visible in the context menu.
        /// </summary>
        public abstract ObservableCollection<IShellCommand> Commands { get; }

        /// <summary>
        /// Gets or sets the minimun width of the context menu.
        /// </summary>
        public float MinimumWidth { get; set; } = 150;

        /// <summary>
        /// Gets or sets the X position of the context menu on the screen.
        /// </summary>
        public float X { get; set; } = 0;

        /// <summary>
        /// Gets or sets the Y position of the context menu on the screen.
        /// </summary>
        public float Y { get; set; } = 22;

        /// <summary>
        /// Executes a command on the context menu.
        /// </summary>
        /// <param name="command"></param>
        public abstract void ExecuteCommand(IShellCommand command);

        /// <summary>
        /// Occurs when the user clicks on a command in the context menu.
        /// </summary>
        /// <param name="sender">The command visual that was clicked.</param>
        /// <param name="e">The event arguments.</param>
        public abstract void ExecuteCommandHandler(object sender, MouseEventArgs e);

        /// <summary>
        /// Occurs when the mouse leaves the context menu visual.
        /// </summary>
        public abstract void MouseLeft();

        /// <summary>
        /// Called when the context menu should become visible and display a list of commands.
        /// </summary>
        /// <param name="commands">The commands to display in the context menu.</param>
        /// <param name="position">The position on the screen to display the context menu.</param>
        public abstract void ShowContextMenu(IList<IShellCommand> commands, Point position);
    }
}