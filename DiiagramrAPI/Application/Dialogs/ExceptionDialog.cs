using System;
using System.Windows;

namespace DiiagramrAPI.Application.Dialogs
{
    public class ExceptionDialog : Dialog
    {
        private Exception _exception;

        public ExceptionDialog(Exception exception)
        {
            _exception = exception;
            Title = _exception.GetType().Name;
            Message = _exception.Message;
            while (_exception.InnerException != null)
            {
                _exception = _exception.InnerException;
                Message += $"\n\nInner Exception: {_exception.GetType().Name}\n Message: {_exception.Message}";
            }
            CommandBarCommands.Add(new DialogCommandBarCommand("Copy Exception", CopyExceptionButtonPressed));
        }

        public override int MaxHeight => 150;

        public override int MaxWidth => 300;

        public override string Title { get; set; }

        public string Message { get; set; }

        public void CopyExceptionButtonPressed()
        {
            Clipboard.SetText($"{Title}: {Message}");
        }
    }
}