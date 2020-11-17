using DiiagramrAPI.Service.Application;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace DiiagramrAPI.Application.Tools
{
    /// <summary>
    /// A basic implementation of <see cref="ContextMenuBase"/>.
    /// </summary>
    public class ContextMenu : ContextMenuBase
    {
        /// <inheritdoc/>
        public override ObservableCollection<IShellCommand> Commands { get; } = new ObservableCollection<IShellCommand>();

        /// <inheritdoc/>
        public override void ExecuteCommandHandler(object sender, MouseEventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;
            if (frameworkElement?.DataContext is IShellCommand command)
            {
                ExecuteCommand(command);
            }
        }

        /// <inheritdoc/>
        public override void ExecuteCommand(IShellCommand command)
        {
            command.Execute(null);
            ClearCommands();
        }

        /// <inheritdoc/>
        public override void MouseLeft()
        {
            ClearCommands();
        }

        /// <inheritdoc/>
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
            NotifyOfPropertyChange(nameof(Commands));
        }

        private void ClearCommands()
        {
            Commands.Clear();
            NotifyOfPropertyChange(nameof(Commands));
        }
    }
}