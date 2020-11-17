using DiiagramrAPI.Application.Tools;
using System.Collections.Generic;
using System.Windows.Input;

namespace DiiagramrAPIUnitTests
{
    internal class FakeHotkeyCommander : IHotkeyHandler
    {
        public List<Key> HandledHotkeyPressed = new List<Key>();
        public bool HandleHotkeyPressReturn;

        public bool HandleHotkeyPress(Key key)
        {
            HandledHotkeyPressed.Add(key);
            return HandleHotkeyPressReturn;
        }
    }
}