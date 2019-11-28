using System;

namespace DiiagramrAPI.Application
{
    public abstract class ShellWindow : ViewModel
    {
        public event Action<ShellWindow> OpenWindow;

        public abstract int MaxHeight { get; }

        public abstract int MaxWidth { get; }

        public abstract string Title { get; }

        protected void OpenOtherWindow(ShellWindow otherWindow)
        {
            OpenWindow?.Invoke(otherWindow);
        }
    }
}