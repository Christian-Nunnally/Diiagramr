using System.Windows.Input;

namespace DiiagramrAPI.Application.Tools
{
    /// <summary>
    /// Interface for interacting with a keyboard.
    /// </summary>
    public interface IKeyboard
    {
        /// <summary>
        /// Whether a particular key is in a pressed state.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>True if <paramref name="key"/> is down.</returns>
        bool IsKeyDown(Key key);
    }
}