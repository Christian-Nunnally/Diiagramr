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
        /// <summary>
        /// An offset to correct for the top left pixel being shifted when WPF windows are maximized.
        /// </summary>
        public const double MaximizedWindowChromeRelativePositionAdjustment = -4;

        private readonly ContextMenuBase _contextMenu;

        private readonly Dictionary<string, IOrderedEnumerable<IToolbarCommand>> _topLevelMenuNameToCommandListMap = new Dictionary<string, IOrderedEnumerable<IToolbarCommand>>();

        /// <summary>
        /// Creates a new instance of <see cref="Toolbar"/>.
        /// </summary>
        /// <param name="toolbarCommandsFactory">A factory that creates a list of commands to add to the toolbar.</param>
        /// <param name="contextMenuFactory">A factory that creates a context menu that is opened when a top level toolbar item is clicked.</param>
        public Toolbar(
            Func<IEnumerable<IToolbarCommand>> toolbarCommandsFactory,
            Func<ContextMenuBase> contextMenuFactory)
        {
            _contextMenu = contextMenuFactory();
            var toolbarCommands = toolbarCommandsFactory().OrderBy(x => x.Weight);
            SetupToolbarCommands(toolbarCommands);
        }

        /// <inheritdoc/>
        public override ObservableCollection<string> TopLevelMenuNames { get; } = new ObservableCollection<string>();

        /// <inheritdoc/>
        public override void OpenContextMenuForTopLevelMenuHandler(object sender, MouseEventArgs e)
        {
            OpenContextMenuFromSender(sender);
        }

        /// <inheritdoc/>
        public override void OpenContextMenuForTopLevelMenu(Point position, string topLevelMenuName)
        {
            var toolbarSubCommands = _topLevelMenuNameToCommandListMap[topLevelMenuName].OfType<IShellCommand>().ToList();
            _contextMenu.ShowContextMenu(toolbarSubCommands, position);
        }

        /// <summary>
        /// Occurs when the mouse enters one of the top level toolbar items.
        /// </summary>
        /// <param name="sender">The top level toolbar item that the mouse entered.</param>
        /// <param name="e">The event arguments.</param>
        public void TopLevelMenuItemMouseEnterHandler(object sender, MouseEventArgs e)
        {
            if (_contextMenu.Commands.Any())
            {
                OpenContextMenuFromSender(sender);
            }
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