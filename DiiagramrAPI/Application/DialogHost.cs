using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace DiiagramrAPI.Application
{
    public class DialogHost : ViewModel
    {
        private readonly Stack<Dialog> _dialogStack = new Stack<Dialog>();
        private Dialog _activeDialog;
        private TimeSpan _lastMouseDownTime;

        public Dialog ActiveDialog
        {
            get => _activeDialog;
            set
            {
                if (_activeDialog != null)
                {
                    _activeDialog.OpenDialog = null;
                }
                _activeDialog = value;
                if (_activeDialog != null)
                {
                    _activeDialog.OpenDialog = OpenDialog;
                }
                IsWindowOpen = ActiveDialog != null;
            }
        }

        public bool IsWindowOpen { get; set; }

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

        public void MouseDownHandler()
        {
            _lastMouseDownTime = DateTime.Now.TimeOfDay;
        }

        public void MouseUpHandler()
        {
            if (DateTime.Now.TimeOfDay.Subtract(_lastMouseDownTime).TotalMilliseconds < 700)
            {
                CloseDialog();
            }
        }
    }
}