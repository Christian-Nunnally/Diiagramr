using System.Collections.Generic;

namespace DiiagramrAPI.Application
{
    /// <summary>
    /// A basic implementation of <see cref="DialogHostBase"/> that manages a stack of dialogs, so that if another dialog
    /// is opened before the current one is closed, the current one will reopen when the use closes the second dialog.
    /// </summary>
    public class DialogHost : DialogHostBase
    {
        private readonly Stack<Dialog> _dialogStack = new Stack<Dialog>();
        private Dialog _activeDialog;

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public override void OpenDialog(Dialog dialog)
        {
            if (ActiveDialog != null)
            {
                _dialogStack.Push(ActiveDialog);
            }
            ActiveDialog = dialog;
        }

        /// <inheritdoc/>
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