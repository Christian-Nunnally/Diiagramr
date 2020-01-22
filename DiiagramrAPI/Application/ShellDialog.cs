using System;

namespace DiiagramrAPI.Application
{
    public abstract class ShellDialog : ViewModel
    {
        public Action<ShellDialog> OpenDialog;

        public abstract int MaxHeight { get; }

        public abstract int MaxWidth { get; }

        public abstract string Title { get; }

        protected void OpenOtherDialog(ShellDialog dialogToOpen)
        {
            OpenDialog?.Invoke(dialogToOpen);
        }
    }
}