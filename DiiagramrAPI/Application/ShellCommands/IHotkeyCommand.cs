using DiiagramrAPI.Service;
using DiiagramrAPI.Service.Application;
using System.Windows.Input;

namespace DiiagramrAPI.Application.ShellCommands
{
    public interface IHotkeyCommand : ISingletonService
    {
        Key Hotkey { get; }

        bool RequiresCtrlModifierKey { get; }

        bool RequiresAltModifierKey { get; }

        bool RequiresShiftModifierKey { get; }
    }

    public static class HotkeyCommandExtensions
    {
        public static void Execute(this IHotkeyCommand toolbarCommand) => toolbarCommand.Execute(null);

        public static void Execute(this IHotkeyCommand toolbarCommand, object parameter)
        {
            if (toolbarCommand is IShellCommand command)
            {
                command.Execute(parameter);
            }
        }
    }
}