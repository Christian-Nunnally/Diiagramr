using Stylet;

namespace DiiagramrAPI.Service.Shell
{
    public interface IScreenDisplayService : IService
    {
        void ShowScreen(IScreen screen);
    }
}