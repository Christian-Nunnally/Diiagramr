using DiiagramrAPI.Application.ShellCommands;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace DiiagramrAPI.Application.Tools
{
    /// <summary>
    /// Quickly interprets hotkeys and invokes the appropriate commands.
    /// </summary>
    public class HotkeyHandler : IHotkeyHandler
    {
        private readonly Dictionary<(Key, bool, bool, bool), IHotkeyCommand> _hotkeyToCommandMap = new Dictionary<(Key, bool, bool, bool), IHotkeyCommand>();
        private readonly IKeyboard _keyboard;

        /// <summary>
        /// Creates a new instance of <see cref="HotkeyHandler"/>.
        /// </summary>
        /// <param name="hotkeyCommandsFactory">A factory that returns a list of of <see cref="IHotkeyCommand"/>s.</param>
        /// <param name="keyboard">A factory that returns an instance of <see cref="IKeyboard"/>.</param>
        public HotkeyHandler(Func<IEnumerable<IHotkeyCommand>> hotkeyCommandsFactory, Func<IKeyboard> keyboard)
        {
            InitializeHotkeyToCommandMap(hotkeyCommandsFactory());
            _keyboard = keyboard();
        }

        /// <inheritdoc/>
        public bool HandleHotkeyPress(Key key)
        {
            var isShiftPressed = _keyboard.IsKeyDown(Key.RightShift) || _keyboard.IsKeyDown(Key.LeftShift);
            var isCtrlPressed = _keyboard.IsKeyDown(Key.RightCtrl) || _keyboard.IsKeyDown(Key.LeftCtrl);
            var isAltPressed = _keyboard.IsKeyDown(Key.RightAlt) || _keyboard.IsKeyDown(Key.LeftAlt);
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