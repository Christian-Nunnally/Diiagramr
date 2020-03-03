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
        public abstract ObservableCollection<IShellCommand> Commands { get; }

        public float MinimumWidth { get; set; } = 150;

        public float X { get; set; } = 0;

        public float Y { get; set; } = 22;

        public abstract void ExecuteCommand(IShellCommand command);

        public abstract void ExecuteCommandHandler(object sender, MouseEventArgs e);

        public abstract void MouseLeft();

        public abstract void ShowContextMenu(IList<IShellCommand> commands, Point position);
    }
}