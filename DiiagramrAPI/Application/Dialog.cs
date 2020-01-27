using System;

namespace DiiagramrAPI.Application
{
    public abstract class Dialog : ViewModel
    {
        public Action<Dialog> OpenDialog;

        public abstract int MaxHeight { get; }

        public abstract int MaxWidth { get; }

        public abstract string Title { get; }

        protected void OpenOtherDialog(Dialog dialogToOpen)
        {
            OpenDialog?.Invoke(dialogToOpen);
        }
    }
}