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

        private readonly ContextMenuBase _contextMenu;

        private readonly Dictionary<string, IOrderedEnumerable<IToolbarCommand>> _topLevelMenuNameToCommandListMap
            = new Dictionary<string, IOrderedEnumerable<IToolbarCommand>>();

        public Toolbar(
            Func<IEnumerable<IToolbarCommand>> toolbarCommandsFactory,
            Func<ContextMenuBase> contextMenuFactory)
        {
            _contextMenu = contextMenuFactory();
            var toolbarCommands = toolbarCommandsFactory().OrderBy(x => x.Weight);
            SetupToolbarCommands(toolbarCommands);
        }

        public override ObservableCollection<string> TopLevelMenuNames { get; } = new ObservableCollection<string>();

        public override void OpenContextMenuForTopLevelMenuHandler(object sender, MouseEventArgs e)
        {
            OpenContextMenuFromSender(sender);
        }

        public void TopLevelMenuItemMouseEnterHandler(object sender, MouseEventArgs e)
        {
            if (_contextMenu.Commands.Any())
            {
                OpenContextMenuFromSender(sender);
            }
        }

        public override void OpenContextMenuForTopLevelMenu(Point position, string topLevelMenuName)
        {
            var toolbarSubCommands = _topLevelMenuNameToCommandListMap[topLevelMenuName].OfType<IShellCommand>().ToList();
            _contextMenu.ShowContextMenu(toolbarSubCommands, position);
        }

        private static SortedDictionary<float, (string, IOrderedEnumerable<IToolbarCommand>)> SortTopLevelMenuItemsByFirstChildWeight(IEnumerable<(string, IOrderedEnumerable<IToolbarCommand>)> topLevelMenuNamesToChildMap)
        {
            var orderedTopLevelMenuNames = new SortedDictionary<float, (string, IOrderedEnumerable<IToolbarCommand>)>();
            foreach (var (parentName, children) in topLevelMenuNamesToChildMap)
            {
                var weight = children.FirstOrDefault().Weight;
                while (orderedTopLevelMenuNames.ContainsKey(weight))
                {
                    weight += 0.0001f;
                }
                orderedTopLevelMenuNames.Add(weight, (parentName, children));
            }

            return orderedTopLevelMenuNames;
        }

        private void OpenContextMenuFromSender(object sender)
        {
            var control = sender as Control;
            if (control?.DataContext is string topLevelMenuName)
            {
                var pointBelowMenuItem = GetPointBelowControl(control);
                OpenContextMenuForTopLevelMenu(pointBelowMenuItem, topLevelMenuName);
            }
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
            var topLevelMenuNames = commands.Select(c => c.ParentName).Distinct();
            var topLevelMenuNamesToChildMap = topLevelMenuNames.Select(parent => FindChildCommandsForParent(parent, commands));
            var orderedTopLevelMenuNames = SortTopLevelMenuItemsByFirstChildWeight(topLevelMenuNamesToChildMap);
            foreach (var (parentName, children) in orderedTopLevelMenuNames.Values)
            {
                _topLevelMenuNameToCommandListMap.Add(parentName, children);
                TopLevelMenuNames.Add(parentName);
            }
        }

        private (string, IOrderedEnumerable<IToolbarCommand>) FindChildCommandsForParent(string parentName, IEnumerable<IToolbarCommand> commands)
        {
            return (parentName, commands.Where(x => x.ParentName == parentName).OrderBy(x => x.Weight));
        }
    }
}