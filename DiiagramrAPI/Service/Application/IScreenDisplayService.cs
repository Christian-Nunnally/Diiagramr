using Stylet;

namespace DiiagramrAPI.Service.Application
{
    public interface IScreenDisplayService : IService
    {
        void ShowScreen(IScreen screen);
    }
}