using DiiagramrAPI.Service;
using DiiagramrAPI.Shell.ShellCommands;
using Stylet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace DiiagramrAPI.Shell.Tools
{
    public class ContextMenuViewModel : Screen
    {
        private readonly Service.ShellCommandFactory _commandManager;

        public ContextMenuViewModel(Func<Service.ShellCommandFactory> commandManagerFactory)
        {
            _commandManager = commandManagerFactory.Invoke();
        }

        public ObservableCollection<IShellCommand> Commands { get; set; } = new ObservableCollection<IShellCommand>();
        public float MinimumWidth { get; set; } = 150;
        public bool Visible { get; set; }
        public float X { get; set; } = 0;
        public float Y { get; set; } = 22;

        public void ExecuteCommand(object sender, MouseEventArgs e)
        {
            var control = sender as FrameworkElement;
            if (control?.DataContext is DiiagramrCommand command)
            {
                Visible = false;
                _commandManager.ExecuteCommand(command);
            }
        }

        public void ShowContextMenu(IList<IShellCommand> commands, Point position)
        {
            X = (float)position.X;
            Y = (float)position.Y;
            Visible = !Visible;
            Commands.Clear();
            commands.ForEach(Commands.Add);
        }

        public void MouseLeft()
        {
            Visible = false;
        }
    }
}
