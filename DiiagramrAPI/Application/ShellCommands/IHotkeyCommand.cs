using System.Windows.Input;

namespace DiiagramrAPI.Application.ShellCommands
{
    public interface IHotkeyCommand
    {
        Key Hotkey { get; }

        bool RequiresCtrlModifierKey { get; }

        bool RequiresAltModifierKey { get; }

        bool RequiresShiftModifierKey { get; }
    }
}