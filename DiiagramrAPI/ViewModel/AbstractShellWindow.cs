using Stylet;
using System;

namespace DiiagramrAPI.ViewModel
{
    public abstract class AbstractShellWindow : Screen
    {
        public abstract int MaxWidth { get; }
        public abstract int MaxHeight { get; }
        public abstract string Title { get; }

        public event Action<AbstractShellWindow> OpenWindow;

        protected void OpenOtherWindow(AbstractShellWindow otherWindow)
        {
            OpenWindow?.Invoke(otherWindow);
        }
    }
}
