using DiiagramrAPI.Service.Application;
using Stylet;
using System;

namespace DiiagramrAPI.Application
{
    public class ScreenHost : Conductor<IScreen>.StackNavigation
    {
        public void ShowScreen(IScreen screen)
        {
            InteractivelyCloseAllScreens(() => ActivateScreen(screen));
        }

        public void InteractivelyCloseAllScreens(Action continuation)
        {
            if (ActiveItem is IUserInputBeforeClosedRequest userInputBeforeClosedRequest)
            {
                userInputBeforeClosedRequest.ContinueIfCanClose(() => InteractivelyCloseNextScreen(continuation));
            }
            else if (ActiveItem is object)
            {
                InteractivelyCloseNextScreen(continuation);
            }
        }

        private void InteractivelyCloseNextScreen(Action continuation)
        {
            ActiveItem.RequestClose();
            InteractivelyCloseAllScreens(continuation);
        }

        private void ActivateScreen(IScreen screen)
        {
            ActiveItem = screen;
            (screen as IShownInShellReaction)?.ShownInShell();
        }
    }
}