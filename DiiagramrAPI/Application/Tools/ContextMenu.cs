using DiiagramrAPI.Application.ShellCommands;
using DiiagramrAPI.Service.Application;
using DiiagramrCore;
using Stylet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace DiiagramrAPI.Application.Tools
{
    public class ContextMenu : Screen
    {
        private readonly ShellCommandFactory _commandManager;

        public ContextMenu(Func<ShellCommandFactory> commandManagerFactory)
        {
            _commandManager = commandManagerFactory.Invoke();
        }

        public ObservableCollection<IShellCommand> Commands { get; } = new ObservableCollection<IShellCommand>();

        public float MinimumWidth { get; set; } = 150;

        public bool Visible { get; set; }

        public float X { get; set; } = 0;

        public float Y { get; set; } = 22;

        public void ExecuteCommand(object sender, MouseEventArgs e)
        {
            var control = sender as FrameworkElement;
            if (control?.DataContext is ShellCommandBase command)
            {
                Visible = false;
                _commandManager.ExecuteCommand(command);
            }
        }

        public void MouseLeft()
        {
            Visible = false;
        }

        public void ShowContextMenu(IList<IShellCommand> commands, Point position)
        {
            X = (float)position.X;
            Y = (float)position.Y;
            Visible = !Visible;
            Commands.Clear();
            commands.ForEach(Commands.Add);
        }
    }
}