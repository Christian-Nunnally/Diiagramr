using DiiagramrAPI.Application.ShellCommands;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace DiiagramrAPI.Application.Tools
{
    /// <summary>
    /// Quickly interprets hotkeys and invokes the appropriate commands.
    /// </summary>
    public class HotkeyCommander : IHotkeyCommander
    {
        private readonly Dictionary<(Key, bool, bool, bool), IHotkeyCommand> _hotkeyToCommandMap = new Dictionary<(Key, bool, bool, bool), IHotkeyCommand>();

        public HotkeyCommander(Func<IEnumerable<IHotkeyCommand>> hotkeyCommandsFactory)
        {
            InitializeHotkeyToCommandMap(hotkeyCommandsFactory());
        }

        public bool HandleHotkeyPress(Key key)
        {
            var isShiftPressed = Keyboard.IsKeyDown(Key.RightShift) || Keyboard.IsKeyDown(Key.LeftShift);
            var isCtrlPressed = Keyboard.IsKeyDown(Key.RightCtrl) || Keyboard.IsKeyDown(Key.LeftCtrl);
            var isAltPressed = Keyboard.IsKeyDown(Key.RightAlt) || Keyboard.IsKeyDown(Key.LeftAlt);
            if (_hotkeyToCommandMap.TryGetValue((key, isCtrlPressed, isShiftPressed, isAltPressed), out var hotkeyCommand))
            {
                hotkeyCommand.Execute();
            }
            return hotkeyCommand is object;
        }

        private void InitializeHotkeyToCommandMap(IEnumerable<IHotkeyCommand> commands)
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