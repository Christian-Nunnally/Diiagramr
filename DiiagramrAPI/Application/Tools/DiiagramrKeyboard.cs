using System.Windows.Input;

namespace DiiagramrAPI.Application.Tools
{
    /// <summary>
    /// Gets information from the real keyboard.
    /// </summary>
    public class DiiagramrKeyboard : IKeyboard
    {
        /// <inheritdoc/>
        public bool IsKeyDown(Key key) => Keyboard.IsKeyDown(key);
    }
}