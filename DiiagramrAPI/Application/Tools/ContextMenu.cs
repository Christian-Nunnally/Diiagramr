using DiiagramrAPI.Application.ShellCommands;
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
    public class ContextMenu : Screen
    {
        public ContextMenu()
        {
        }

        public ObservableCollection<IShellCommand> Commands { get; } = new ObservableCollection<IShellCommand>();

        public float MinimumWidth { get; set; } = 150;

        public bool Visible { get; set; }

        public float X { get; set; } = 0;

        public float Y { get; set; } = 22;

        public bool CloseWhenMouseLeaves { get; set; } = true;

        public void ExecuteCommand(object sender, MouseEventArgs e)
        {
            var control = sender as FrameworkElement;
            if (control?.DataContext is ShellCommandBase command)
            {
                Visible = false;
                command.Execute();
            }
        }

        public void MouseLeft()
        {
            if (CloseWhenMouseLeaves)
            {
                Visible = false;
            }
        }

        public void ShowContextMenu(IList<IShellCommand> commands, Point position)
        {
            X = (float)position.X;
            Y = (float)position.Y;
            Visible = true;
            Commands.Clear();
            foreach (var command in commands)
            {
                command.CanExecute();
                Commands.Add(command);
            }
        }
    }
}