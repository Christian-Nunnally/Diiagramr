using System.Collections.Generic;

namespace DiiagramrAPI.Application
{
    public class DialogHost : DialogHostBase
    {
        private readonly Stack<Dialog> _dialogStack = new Stack<Dialog>();
        private Dialog _activeDialog;

        public override Dialog ActiveDialog
        {
            get => _activeDialog;
            set
            {
                SetActiveDialogsHostToNull();
                _activeDialog = value;
                SetActiveDialogsHostToThis();
            }
        }

        public override void OpenDialog(Dialog dialog)
        {
            if (ActiveDialog != null)
            {
                _dialogStack.Push(ActiveDialog);
            }
            ActiveDialog = dialog;
        }

        public override void CloseDialog()
        {
            ActiveDialog = _dialogStack.Count > 0 ? _dialogStack.Pop() : null;
        }

        private void SetActiveDialogsHostToThis()
        {
            if (ActiveDialog != null)
            {
                ActiveDialog.CurrentDialogHost = this;
            }
        }

        private void SetActiveDialogsHostToNull()
        {
            if (ActiveDialog != null)
            {
                ActiveDialog.CurrentDialogHost = null;
            }
        }
    }
}