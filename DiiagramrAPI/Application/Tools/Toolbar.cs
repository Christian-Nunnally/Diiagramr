using DiiagramrAPI.Application.ShellCommands;
using Stylet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DiiagramrAPI.Application.Tools
{
    public class Toolbar : Screen
    {
        private readonly ContextMenu _contextMenu;

        // TODO: This is now the owner of the command manager. We should probaby move ownership to another class and get commands from somewhere else.
        private readonly ShellCommands.CommandManager _commandManager;

        public Toolbar(Func<ShellCommands.CommandManager> commandManagerFactory, Func<ContextMenu> contextMenuFactory)
        {
            _contextMenu = contextMenuFactory();
            _commandManager = commandManagerFactory();
            var toolbarCommands = _commandManager.Commands.OfType<ToolBarCommand>();
            SetupToolbarCommands(toolbarCommands);
        }

        public ObservableCollection<TopLevelToolBarCommand> TopLevelMenuItems { get; } = new ObservableCollection<TopLevelToolBarCommand>();

        public void ExecuteCommandHandler(object sender, MouseEventArgs e)
        {
            var control = sender as Control;
            if (control?.DataContext is ShellCommandBase command)
            {
                var shellRelativePosition = control.TransformToAncestor(View);
                var correctedRelativePosition = shellRelativePosition.Transform(new Point(0, 2));

                if (View is Window window)
                {
                    if (window.WindowState == WindowState.Maximized)
                    {
                        correctedRelativePosition = new Point(correctedRelativePosition.X + Shell.MaximizedWindowChromeRelativePositionAdjustment, correctedRelativePosition.Y + Shell.MaximizedWindowChromeRelativePositionAdjustment);
                    }
                }

                var pointBelowMenuItem = new Point(correctedRelativePosition.X, correctedRelativePosition.Y + 19);
                _contextMenu.ShowContextMenu(command.SubCommandItems, pointBelowMenuItem);
            }
        }

        private void SetupToolbarCommands(IEnumerable<ToolBarCommand> commands)
        {
            var topLevelMenuItems = commands.OfType<TopLevelToolBarCommand>();
            var nonTopLevelMenuItems = commands.Where(x => x.Parent != null);
            foreach (var topLevelMenuItem in topLevelMenuItems.OrderBy(x => x.Weight))
            {
                TopLevelMenuItems.Add(topLevelMenuItem);
                foreach (var subMenuItem in nonTopLevelMenuItems.Where(x => x.Parent == topLevelMenuItem.Name).OrderBy(x => x.Weight))
                {
                    topLevelMenuItem.SubCommandItems.Add(subMenuItem);
                }
            }
        }
    }
}