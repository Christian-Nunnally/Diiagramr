using DiiagramrAPI.Application.ShellCommands;
using DiiagramrAPI.Service.Application;
using DiiagramrCore;
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
        private readonly Dictionary<(Key, bool, bool, bool), IToolbarCommand> _hotkeyToCommandMap = new Dictionary<(Key, bool, bool, bool), IToolbarCommand>();
        private readonly Dictionary<IToolbarCommand, List<IToolbarCommand>> _topLevelCommandToSubCommandMap = new Dictionary<IToolbarCommand, List<IToolbarCommand>>();

        // TODO: This is now the owner of the command manager. We should probaby move ownership to another class and get commands from somewhere else.
        private readonly ShellCommands.CommandManager _commandManager;

        public Toolbar(Func<ShellCommands.CommandManager> commandManagerFactory, Func<ContextMenu> contextMenuFactory)
        {
            _contextMenu = contextMenuFactory();
            _commandManager = commandManagerFactory();
            var toolbarCommands = _commandManager.ToolbarCommands.OrderBy(x => x.Weight);
            SetupToolbarCommands(toolbarCommands);
        }

        public ObservableCollection<IToolbarCommand> TopLevelMenuItems { get; } = new ObservableCollection<IToolbarCommand>();

        public void ExecuteCommandHandler(object sender, MouseEventArgs e)
        {
            var control = sender as Control;
            if (control?.DataContext is IToolbarCommand command)
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
                _contextMenu.ShowContextMenu(_topLevelCommandToSubCommandMap[command].OfType<IShellCommand>().ToList(), pointBelowMenuItem);
            }
        }

        public bool HandleHotkeyPress(bool ctrlKey, bool shiftKey, bool altKey, Key key)
        {
            if (_hotkeyToCommandMap.TryGetValue((key, ctrlKey, shiftKey, altKey), out var command))
            {
                command.Execute(null);
                return true;
            }
            return false;
        }

        private void SetupToolbarCommands(IEnumerable<IToolbarCommand> commands)
        {
            InitializeHotkeyToCommandMap(commands);

            commands.Where(c => c.ParentName is null).OrderBy(w => w.Weight).Reverse().ForEach(AddNewTopLevelCommand);
            var nonTopLevelMenuItems = commands.Where(x => x.ParentName is object);
            foreach (var topLevelMenuItem in TopLevelMenuItems)
            {
                var childCommands = nonTopLevelMenuItems.Where(x => x.ParentName == topLevelMenuItem.Name);
                var orderedChildCommands = childCommands.OrderBy(x => x.Weight);
                foreach (var childCommand in orderedChildCommands)
                {
                    _topLevelCommandToSubCommandMap[topLevelMenuItem].Add(childCommand);
                }
            }
        }

        private void AddNewTopLevelCommand(IToolbarCommand command)
        {
            _topLevelCommandToSubCommandMap.Add(command, new List<IToolbarCommand>());
            TopLevelMenuItems.Add(command);
        }

        private void InitializeHotkeyToCommandMap(IEnumerable<IToolbarCommand> commands)
        {
            foreach (var command in commands)
            {
                if (command is IHotkeyCommand hotkeyCommand && hotkeyCommand.Hotkey != Key.None)
                {
                    var hotkeyTuple = (hotkeyCommand.Hotkey,
                        hotkeyCommand.RequiresCtrlModifierKey,
                        hotkeyCommand.RequiresShiftModifierKey,
                        hotkeyCommand.RequiresAltModifierKey);
                    _hotkeyToCommandMap.Add(hotkeyTuple, command);
                }
            }
        }
    }
}