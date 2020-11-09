using System.Windows.Input;

namespace DiiagramrAPI.Application.Tools
{
    public interface IKeyboard
    {
        bool IsKeyDown(Key key);
    }
}