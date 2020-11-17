using DiiagramrAPI.Service;
using DiiagramrAPI.Service.Application;
using System.Windows.Input;

namespace DiiagramrAPI.Application.ShellCommands
{
    /// <summary>
    /// A command that can be executed via a hotkey.
    /// </summary>
    public interface IHotkeyCommand : ISingletonService
    {
        /// <summary>
        /// The hotkey assigned to this command.
        /// </summary>
        Key Hotkey { get; }

        /// <summary>
        /// Gets whether the control key has to be pressed in conjuction with the <see cref="Hotkey"/> in order to execute this command.
        /// </summary>
        bool RequiresCtrlModifierKey { get; }

        /// <summary>
        /// Gets whether the alt key has to be pressed in conjuction with the <see cref="Hotkey"/> in order to execute this command.
        /// </summary>
        bool RequiresAltModifierKey { get; }

        /// <summary>
        /// Gets whether the shift key has to be pressed in conjuction with the <see cref="Hotkey"/> in order to execute this command.
        /// </summary>
        bool RequiresShiftModifierKey { get; }
    }

    /// <summary>
    /// Useful extension methods to directly execute an <see cref="IHotkeyCommand"/>.
    /// </summary>
    public static class HotkeyCommandExtensions
    {
        /// <summary>
        /// Executes the given hotkey command with a null argument.
        /// </summary>
        /// <param name="hotkeyCommand">The command to execute.</param>
        public static void Execute(this IHotkeyCommand hotkeyCommand) => hotkeyCommand.Execute(null);

        /// <summary>
        /// Executes a hotkey command.
        /// </summary>
        /// <param name="hotkeyCommand">The command to execute.</param>
        /// <param name="parameter">The parameter to pass in to the command.</param>
        public static void Execute(this IHotkeyCommand hotkeyCommand, object parameter)
        {
            if (hotkeyCommand is IShellCommand command)
            {
                command.Execute(parameter);
            }
        }
    }
}