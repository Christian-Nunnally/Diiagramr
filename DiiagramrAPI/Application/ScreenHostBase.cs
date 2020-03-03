using Stylet;
using System;

namespace DiiagramrAPI.Application
{
    public abstract class ScreenHostBase : Conductor<IScreen>.StackNavigation
    {
        public abstract void ShowScreen(IScreen screen);

        public abstract void InteractivelyCloseAllScreens(Action continuation);
    }
}