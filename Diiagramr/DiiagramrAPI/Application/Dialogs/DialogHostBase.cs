using Stylet;
using System.Windows;
using System.Windows.Input;
using static DiiagramrAPI.Application.Dialog;

namespace DiiagramrAPI.Application
{
    /// <summary>
    /// A UI layer capable of showing 'model' dialogs.
    /// </summary>
    public abstract class DialogHostBase : Screen
    {
        /// <summary>
        /// The currently visible <see cref="Dialog"/>.
        /// </summary>
        public abstract Dialog ActiveDialog { get; set; }

        /// <summary>
        /// Shows a dialog.
        /// </summary>
        /// <param name="dialog">The dialog to open.</param>
        public abstract void OpenDialog(Dialog dialog);

        /// <summary>
        /// Closes the currently visible dialog.
        /// </summary>
        public abstract void CloseDialog();

        /// <summary>
        /// Occurs when the user clicks in the dialog.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguemnts.</param>
        public void MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// Occurs when the user clicks a command bar button in the dialog.
        /// </summary>
        /// <param name="sender">The command bar button clicked.</param>
        /// <param name="e">The event arguemnts.</param>
        public void CommandBarActionClickedHandler(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is DialogCommandBarCommand command)
            {
                command.Action();
            }
        }
    }
}