using System;

namespace DiiagramrAPI.Shell
{
    public abstract class AbstractShellWindow : ViewModel
    {
        public event Action<AbstractShellWindow> OpenWindow;

        public abstract int MaxHeight { get; }
        public abstract int MaxWidth { get; }
        public abstract string Title { get; }

        protected void OpenOtherWindow(AbstractShellWindow otherWindow)
        {
            OpenWindow?.Invoke(otherWindow);
        }
    }
}
