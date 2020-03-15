using DiiagramrAPI.Application.Dialogs;
using System;

namespace DiiagramrAPI.Application.ShellCommands.ToolCommands
{
    public class OpenLibraryManagerDialogCommand : ShellCommandBase, IToolbarCommand
    {
        private readonly LibraryManagerDialog _libraryManagerDialog;
        private readonly DialogHostBase _dialogHost;

        public OpenLibraryManagerDialogCommand(
            Func<LibraryManagerDialog> startScreenViewModelFactory,
            Func<DialogHostBase> dialogHostFactory)
        {
            _libraryManagerDialog = startScreenViewModelFactory();
            _dialogHost = dialogHostFactory();
        }

        public override string Name => "Manage Libraries";

        public string ParentName => "Tools";

        public float Weight => .1f;

        protected override bool CanExecuteInternal()
        {
            return true;
        }

        protected override void ExecuteInternal(object parameter)
        {
            _dialogHost.OpenDialog(_libraryManagerDialog);
        }
    }
}