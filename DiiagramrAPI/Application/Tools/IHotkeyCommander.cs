using DiiagramrAPI.Service;
using System.Windows.Input;

namespace DiiagramrAPI.Application.Tools
{
    public interface IHotkeyCommander : ISingletonService
    {
        bool HandleHotkeyPress(Key key);
    }
}