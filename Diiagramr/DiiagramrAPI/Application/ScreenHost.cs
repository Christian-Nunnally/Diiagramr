using DiiagramrAPI.Service.Application;
using Stylet;
using System;

namespace DiiagramrAPI.Application
{
    /// <summary>
    /// Provides a simple API for showing screens in the UI.
    /// </summary>
    public class ScreenHost : ScreenHostBase
    {
        /// <inheritdoc/>
        public override void ShowScreen(IScreen screen)
        {
            if (screen == ActiveItem) return;
            InteractivelyCloseAllScreens(() => ActivateScreen(screen));
        }

        /// <inheritdoc/>
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