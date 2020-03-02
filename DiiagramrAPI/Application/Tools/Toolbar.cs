using DiiagramrAPI.Application.ShellCommands;
using DiiagramrAPI.Service.Application;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DiiagramrAPI.Application.Tools
{
    /// <summary>
    /// The toolbar of the shell. Contains a list of top level commands, each of which can contain child commands. All commands have weights to determine order.
    /// </summary>
    public class Toolbar : ToolbarBase
    {
        public const double MaximizedWindowChromeRelativePositionAdjustment = -4;

        private readonly ContextMenu _contextMenu;

        private readonly Dictionary<IToolbarCommand, IOrderedEnumerable<IToolbarCommand>> _topLevelCommandToSubCommandMap
            = new Dictionary<IToolbarCommand, IOrderedEnumerable<IToolbarCommand>>();

        public Toolbar(Func<IEnumerable<IToolbarCommand>> toolbarCommandsFactory, Func<ContextMenu> contextMenuFactory)
        {
            _contextMenu = contextMenuFactory();
            var toolbarCommands = toolbarCommandsFactory().OrderBy(x => x.Weight);
            SetupToolbarCommands(toolbarCommands);
        }

        public override ObservableCollection<IToolbarCommand> TopLevelMenuItems { get; } = new ObservableCollection<IToolbarCommand>();

        public override void ExecuteCommandHandler(object sender, MouseEventArgs e)
        {
            var control = sender as Control;
            if (control?.DataContext is IToolbarCommand command)
            {
                var pointBelowMenuItem = GetPointBelowControl(control);
                var toolbarSubCommands = _topLevelCommandToSubCommandMap[command].OfType<IShellCommand>().ToList();
                _contextMenu.ShowContextMenu(toolbarSubCommands, pointBelowMenuItem);
            }
        }

        private static IEnumerable<IToolbarCommand> GetOrderedToolbarCommandsWithNoParent(IEnumerable<IToolbarCommand> commands)
        {
            return commands
                .Where(c => c.ParentName is null)
                .OrderByDescending(w => w.Weight);
        }

        private static IEnumerable<IToolbarCommand> GetToolbarCommandWithAParent(IEnumerable<IToolbarCommand> commands)
        {
            return commands.Where(x => x.ParentName is object);
        }

        private Point GetPointBelowControl(Control control)
        {
            var shellRelativePosition = control.TransformToAncestor(View);
            var correctedRelativePosition = shellRelativePosition.Transform(new Point(0, 2));
            correctedRelativePosition = ShiftPointIfWindowMaximized(correctedRelativePosition);
            return new Point(correctedRelativePosition.X, correctedRelativePosition.Y + 19);
        }

        private Point ShiftPointIfWindowMaximized(Point correctedRelativePosition)
        {
            if (View is Window window && window.WindowState == WindowState.Maximized)
            {
                var correctedX = correctedRelativePosition.X + MaximizedWindowChromeRelativePositionAdjustment;
                var correctedY = correctedRelativePosition.Y + MaximizedWindowChromeRelativePositionAdjustment;
                return new Point(correctedX, correctedY);
            }
            return correctedRelativePosition;
        }

        private void SetupToolbarCommands(IEnumerable<IToolbarCommand> commands)
        {
            var nonTopLevelMenuItems = GetToolbarCommandWithAParent(commands);
            var topLevelCommandToChildMap = GetOrderedToolbarCommandsWithNoParent(commands)
                .Select(parent => FindChildCommandsForParent(parent, nonTopLevelMenuItems));
            foreach (var (parent, children) in topLevelCommandToChildMap)
            {
                _topLevelCommandToSubCommandMap.Add(parent, children);
                TopLevelMenuItems.Add(parent);
            }
        }

        private (IToolbarCommand, IOrderedEnumerable<IToolbarCommand>) FindChildCommandsForParent(IToolbarCommand parent, IEnumerable<IToolbarCommand> commands)
        {
            return (parent, commands.Where(x => x.ParentName == parent.Name).OrderBy(x => x.Weight));
        }
    }
}