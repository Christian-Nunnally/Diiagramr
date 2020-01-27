using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace DiiagramrAPI.Application
{
    public abstract class Dialog : ViewModel
    {
        public Action<Dialog> OpenDialogAction;
        public Action CloseDialogAction;

        public abstract int MaxHeight { get; }

        public abstract int MaxWidth { get; }

        public abstract string Title { get; set; }

        public bool HasCommandBarCommands => CommandBarCommands.Any();

        public ObservableCollection<DialogCommandBarCommand> CommandBarCommands { get; set; } = new ObservableCollection<DialogCommandBarCommand>();

        protected void OpenOtherDialog(Dialog dialogToOpen)
        {
            OpenDialogAction?.Invoke(dialogToOpen);
        }

        protected void CloseDialog()
        {
            CloseDialogAction?.Invoke();
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