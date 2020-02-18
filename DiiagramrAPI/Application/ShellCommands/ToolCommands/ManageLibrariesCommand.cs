using DiiagramrAPI.Application.Dialogs;
using System;

namespace DiiagramrAPI.Application.ShellCommands.ToolCommands
{
    public class ManageLibrariesCommand : ShellCommandBase, IToolbarCommand
    {
        private readonly LibraryManagerDialog _libraryManagerDialog;
        private readonly DialogHost _dialogHost;

        public ManageLibrariesCommand(
            Func<LibraryManagerDialog> startScreenViewModelFactory,
            Func<DialogHost> dialogHostFactory)
        {
            _libraryManagerDialog = startScreenViewModelFactory();
            _dialogHost = dialogHostFactory();
        }

        public override string Name => "Libraries";

        public string ParentName => "Tools";

        public float Weight => .5f;

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