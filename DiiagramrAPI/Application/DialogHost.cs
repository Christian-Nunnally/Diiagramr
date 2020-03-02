using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using static DiiagramrAPI.Application.Dialog;

namespace DiiagramrAPI.Application
{
    public class DialogHost : ViewModel
    {
        private readonly Stack<Dialog> _dialogStack = new Stack<Dialog>();
        private Dialog _activeDialog;

        public Dialog ActiveDialog
        {
            get => _activeDialog;
            set
            {
                SetActiveDialogOpenAndCloseHandlers(null, null);
                _activeDialog = value;
                SetActiveDialogOpenAndCloseHandlers(OpenDialog, CloseDialog);
                IsDialogOpen = _activeDialog != null;
            }
        }

        public bool IsDialogOpen { get; set; }

        public void OpenDialog(Dialog dialog)
        {
            if (ActiveDialog != null)
            {
                _dialogStack.Push(ActiveDialog);
            }
            ActiveDialog = dialog;
        }

        public void CloseDialog()
        {
            ActiveDialog = _dialogStack.Count > 0 ? _dialogStack.Pop() : null;
        }

        public void MouseDownHandled(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        public void CommandBarActionClicked(object sender, MouseButtonEventArgs e)
        {
            var dataContext = (sender as FrameworkElement)?.DataContext;
            var choiceAction = (dataContext as DialogCommandBarCommand)?.Action;
            choiceAction?.Invoke();
        }

        private void SetActiveDialogOpenAndCloseHandlers(Action<Dialog> openDialogAction, Action closeDialogAction)
        {
            if (ActiveDialog != null)
            {
                ActiveDialog.OpenDialogAction = openDialogAction;
                ActiveDialog.CloseDialogAction = closeDialogAction;
            }
        }
    }
}