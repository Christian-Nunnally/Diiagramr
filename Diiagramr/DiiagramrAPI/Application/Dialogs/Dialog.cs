using Stylet;
using System;
using System.Collections.ObjectModel;

namespace DiiagramrAPI.Application
{
    /// <summary>
    /// An abstract representation of a dialog so that all Diiagramr dialogs can have a common look and feel.
    /// </summary>
    public abstract class Dialog : Screen
    {
        /// <summary>
        /// The <see cref="DialogHost"/> object that is displaying this dialog, if any.
        /// </summary>
        public DialogHost CurrentDialogHost { get; set; }

        /// <summary>
        /// The maximum height of the dialog.
        /// </summary>
        public abstract int MaxHeight { get; }

        /// <summary>
        /// The maximum width of the dialog.
        /// </summary>
        public abstract int MaxWidth { get; }

        /// <summary>
        /// The title of the dialog, displayed above the dialog when it is shown.
        /// </summary>
        public abstract string Title { get; set; }

        /// <summary>
        /// A list of commands that appear at the bottom of the dialog as simple buttons.
        /// </summary>
        public ObservableCollection<DialogCommandBarCommand> CommandBarCommands { get; set; } = new ObservableCollection<DialogCommandBarCommand>();

        /// <summary>
        /// Opens another dialog. Allows <see cref="Dialog"/> implementations to open sub-dialogs.
        /// </summary>
        /// <param name="dialogToOpen">The other dialog to open.</param>
        protected void OpenDialog(Dialog dialogToOpen)
        {
            CurrentDialogHost.OpenDialog(dialogToOpen);
        }

        /// <summary>
        /// Closes the dialog. Allows <see cref="Dialog"/> implementations to close themselves.
        /// </summary>
        protected void CloseDialog()
        {
            CurrentDialogHost.CloseDialog();
        }

        /// <summary>
        /// Helper class to wrap a custom label around a command for use in the dialog command bar.
        /// </summary>
        public class DialogCommandBarCommand
        {
            /// <summary>
            /// Creates a new instance of <see cref="DialogCommandBarCommand"/>.
            /// </summary>
            /// <param name="label">The text to display on the label of the command bar button.</param>
            /// <param name="action">The action to execute when the command bar button is clicked.</param>
            internal DialogCommandBarCommand(string label, Action action)
            {
                Label = label;
                Action = action;
            }

            /// <summary>
            /// Gets or sets the text to display on the label of the command bar button.
            /// </summary>
            public string Label { get; set; }

            /// <summary>
            /// Gets or sets the action to execute when the command bar button is clicked.
            /// </summary>
            public Action Action { get; set; }
        }
    }
}