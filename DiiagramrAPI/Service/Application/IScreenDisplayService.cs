using Stylet;

namespace DiiagramrAPI.Service.Application
{
    public interface IScreenDisplayService : ISingletonService
    {
        void ShowScreen(IScreen screen);
    }
}