using System.Windows.Input;

namespace DiiagramrAPI.Application.Tools
{
    public class DiiagramrKeyboard : IKeyboard
    {
        public bool IsKeyDown(Key key) => Keyboard.IsKeyDown(key);
    }
}