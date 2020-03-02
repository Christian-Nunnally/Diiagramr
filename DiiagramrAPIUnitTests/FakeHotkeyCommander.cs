using DiiagramrAPI.Application.Tools;
using System.Windows.Input;

namespace DiiagramrAPIUnitTests
{
    internal class FakeHotkeyCommander : IHotkeyCommander
    {
        public bool HandleHotkeyPress(Key key)
        {
            return true;
        }
    }
}