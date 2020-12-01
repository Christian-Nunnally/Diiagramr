using DiiagramrAPI.Application;
using Stylet;
using System;

namespace DiiagramrAPIUnitTests
{
    internal class FakeScreenHost : ScreenHostBase
    {
        public event Action InteractivelyCloseAllScreensAction;

        public override void InteractivelyCloseAllScreens(Action continuation)
        {
            InteractivelyCloseAllScreensAction?.Invoke();
        }

        public override void ShowScreen(IScreen screen)
        {
        }
    }
}