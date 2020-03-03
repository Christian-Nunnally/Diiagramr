using DiiagramrAPI.Service.Application;
using Stylet;
using System;

namespace DiiagramrAPI.Application
{
    public class ScreenHost : ScreenHostBase
    {
        public override void ShowScreen(IScreen screen)
        {
            InteractivelyCloseAllScreens(() => ActivateScreen(screen));
        }

        public override void InteractivelyCloseAllScreens(Action continuation)
        {
            if (ActiveItem is IUserInputBeforeClosedRequest userInputBeforeClosedRequest)
            {
                userInputBeforeClosedRequest.ContinueIfCanClose(() => InteractivelyCloseNextScreen(continuation));
            }
            else if (ActiveItem is object)
            {
                InteractivelyCloseNextScreen(continuation);
            }
            else
            {
                continuation();
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