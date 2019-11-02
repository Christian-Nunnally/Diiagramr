using DiiagramrAPI.Application.Tools;
using System;

namespace DiiagramrAPI.Application.ShellCommands.ToolCommands
{
    public class ManageLibrariesCommand : ToolBarCommand
    {
        private readonly LibraryManagerWindow _libraryManagerWindowViewModel;

        public ManageLibrariesCommand(Func<LibraryManagerWindow> startScreenViewModelFactory)
        {
            _libraryManagerWindowViewModel = startScreenViewModelFactory.Invoke();
        }

        public override string Name => "Libraries";
        public override string Parent => "Tools";
        public override float Weight => .5f;

        internal override void ExecuteInternal(IApplicationShell shell, object parameter)
        {
            shell.OpenWindow(_libraryManagerWindowViewModel);
        }
    }
}