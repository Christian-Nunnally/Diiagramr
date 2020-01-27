using DiiagramrAPI.Service.Application;
using Stylet;

namespace DiiagramrAPI.Application
{
    public class ScreenHost : Conductor<IScreen>.StackNavigation
    {
        public void ShowScreen(IScreen screen)
        {
            CloseCurrentScreens();
            ActiveItem = screen;
            if (screen is IShownInShellReaction reaction)
            {
                reaction.ShownInShell();
            }
        }

        public void CloseCurrentScreens()
        {
            while (ActiveItem != null)
            {
                ActiveItem.RequestClose();
            }
        }
    }
}