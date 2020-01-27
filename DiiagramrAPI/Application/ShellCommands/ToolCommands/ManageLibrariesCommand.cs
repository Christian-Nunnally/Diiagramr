using DiiagramrAPI.Application.Dialogs;
using System;

namespace DiiagramrAPI.Application.ShellCommands.ToolCommands
{
    public class ManageLibrariesCommand : ToolBarCommand
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

        public override string Parent => "Tools";

        public override float Weight => .5f;

        protected override void ExecuteInternal(object parameter)
        {
            _dialogHost.OpenDialog(_libraryManagerDialog);
        }
    }
}