using System;
using System.Collections.ObjectModel;

namespace DiiagramrAPI.Application
{
    public abstract class Dialog : ViewModel
    {
        public DialogHost CurrentDialogHost { get; set; }

        public abstract int MaxHeight { get; }

        public abstract int MaxWidth { get; }

        public abstract string Title { get; set; }

        public ObservableCollection<DialogCommandBarCommand> CommandBarCommands { get; set; } = new ObservableCollection<DialogCommandBarCommand>();

        protected void OpenDialog(Dialog dialogToOpen)
        {
            CurrentDialogHost.OpenDialog(dialogToOpen);
        }

        protected void CloseDialog()
        {
            CurrentDialogHost.CloseDialog();
        }

        public class DialogCommandBarCommand
        {
            internal DialogCommandBarCommand(string label, Action action)
            {
                Label = label;
                Action = action;
            }

            public string Label { get; set; }

            public Action Action { get; set; }
        }
    }
}