using DiiagramrAPI.Application.Dialogs;
using System;

namespace DiiagramrAPI.Application.ShellCommands.ToolCommands
{
    /// <summary>
    /// Command that opens the library manager dialog window.
    /// </summary>
    public class OpenLibraryManagerDialogCommand : ShellCommandBase, IToolbarCommand
    {
        private readonly LibraryManagerDialog _libraryManagerDialog;
        private readonly DialogHostBase _dialogHost;

        /// <summary>
        /// Creates a new instance of <see cref="OpenLibraryManagerDialogCommand"/>
        /// </summary>
        /// <param name="libraryManagerDialogFactory">Factory to get an instance of an <see cref="LibraryManagerDialog"/>.</param>
        /// <param name="dialogHostFactory">Factory to get an instance of an <see cref="DialogHostBase"/>.</param>
        public OpenLibraryManagerDialogCommand(
            Func<LibraryManagerDialog> libraryManagerDialogFactory,
            Func<DialogHostBase> dialogHostFactory)
        {
            _libraryManagerDialog = libraryManagerDialogFactory();
            _dialogHost = dialogHostFactory();
        }

        /// <inheritdoc/>
        public override string Name => "Manage Libraries";

        /// <inheritdoc/>
        public string ParentName => "Tools";

        /// <inheritdoc/>
        public float Weight => .1f;

        /// <inheritdoc/>
        protected override bool CanExecuteInternal()
        {
            return true;
        }

        /// <inheritdoc/>
        protected override void ExecuteInternal(object parameter)
        {
            _dialogHost.OpenDialog(_libraryManagerDialog);
        }
    }
}