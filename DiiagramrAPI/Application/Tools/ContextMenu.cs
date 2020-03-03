using DiiagramrAPI.Service.Application;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace DiiagramrAPI.Application.Tools
{
    public class ContextMenu : ContextMenuBase
    {
        public override ObservableCollection<IShellCommand> Commands { get; } = new ObservableCollection<IShellCommand>();

        public override void ExecuteCommandHandler(object sender, MouseEventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;
            if (frameworkElement?.DataContext is IShellCommand command)
            {
                ExecuteCommand(command);
            }
        }

        public override void ExecuteCommand(IShellCommand command)
        {
            command.Execute(null);
            Commands.Clear();
        }

        public override void MouseLeft()
        {
            Commands.Clear();
        }

        public override void ShowContextMenu(IList<IShellCommand> commands, Point position)
        {
            X = (float)position.X;
            Y = (float)position.Y;
            Commands.Clear();
            foreach (var command in commands)
            {
                command.CanExecute();
                Commands.Add(command);
            }
        }
    }
}