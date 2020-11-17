using DiiagramrAPI.Service;
using System.Windows.Input;

namespace DiiagramrAPI.Application.Tools
{
    /// <summary>
    /// Handles hotkeys.
    /// </summary>
    public interface IHotkeyHandler : ISingletonService
    {
        /// <summary>
        /// Called when a hotkey is detected by some other mechanism.
        /// </summary>
        /// <param name="key">The key that was pressed.</param>
        /// <returns>True is the key press was handle.</returns>
        bool HandleHotkeyPress(Key key);
    }
}