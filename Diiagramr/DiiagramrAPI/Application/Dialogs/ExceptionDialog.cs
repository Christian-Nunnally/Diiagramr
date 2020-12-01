using System;
using System.Windows;

namespace DiiagramrAPI.Application.Dialogs
{
    /// <summary>
    /// A dialog that displays an <see cref="Exception"/>
    /// </summary>
    public class ExceptionDialog : Dialog
    {
        private Exception _exception;

        /// <summary>
        /// Creates a new instance of <see cref="ExceptionDialog"/>.
        /// </summary>
        /// <param name="exception">The exception to display the information of in the dialog.</param>
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

        /// <inheritdoc/>
        public override int MaxHeight => 150;

        /// <inheritdoc/>
        public override int MaxWidth => 300;

        /// <inheritdoc/>
        public override string Title { get; set; }

        /// <summary>
        /// A message to display in the dialog.
        /// </summary>
        public string Message { get; set; }

        private void CopyExceptionButtonPressed()
        {
            Clipboard.SetText($"{Title}: {Message}");
        }
    }
}