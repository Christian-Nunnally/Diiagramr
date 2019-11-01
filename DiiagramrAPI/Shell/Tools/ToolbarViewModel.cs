using DiiagramrAPI.Service.Commands;
using Stylet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DiiagramrAPI.Shell.Tools
{
    public class ToolbarViewModel : Screen
    {
        private readonly Service.ShellCommandFactory _commandManager;

        public ObservableCollection<TopLevelToolBarCommand> TopLevelMenuItems { get; set; }

        public ToolbarViewModel(Func<Service.ShellCommandFactory> commandManagerFactory)
        {
            _commandManager = commandManagerFactory.Invoke();
            var toolbarCommands = _commandManager.Commands.OfType<ToolBarCommand>();
            SetupToolbarCommands(toolbarCommands);
        }

        private void SetupToolbarCommands(IEnumerable<ToolBarCommand> commands)
        {
            TopLevelMenuItems = new ObservableCollection<TopLevelToolBarCommand>();
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

        public void ExecuteCommandHandler(object sender, MouseEventArgs e)
        {
            var control = sender as Control;
            if (control?.DataContext is DiiagramrCommand command)
            {
                var shellRelativePosition = control.TransformToAncestor(View);
                var correctedRelativePosition = shellRelativePosition.Transform(new Point(0, 2));

                if (View is Window window)
                {
                    if (window.WindowState == WindowState.Maximized)
                    {
                        correctedRelativePosition = new Point(correctedRelativePosition.X + ShellViewModel.MaximizedWindowChromeRelativePositionAdjustment, correctedRelativePosition.Y + ShellViewModel.MaximizedWindowChromeRelativePositionAdjustment);
                    }
                }
                _commandManager.ExecuteCommand(command, correctedRelativePosition);
            }
        }
    }
}
