using DiiagramrAPI.Shell;
using DiiagramrAPI.Shell.Tools;
using System;

namespace DiiagramrAPI.Shell.ShellCommands.ToolCommands
{
    public class ManageLibrariesCommand : ToolBarCommand
    {
        private readonly LibraryManagerWindowViewModel _libraryManagerWindowViewModel;

        public ManageLibrariesCommand(Func<LibraryManagerWindowViewModel> startScreenViewModelFactory)
        {
            _libraryManagerWindowViewModel = startScreenViewModelFactory.Invoke();
        }

        public override string Name => "Libraries";
        public override string Parent => "Tools";
        public override float Weight => .5f;

        internal override void ExecuteInternal(IShell shell, object parameter)
        {
            shell.OpenWindow(_libraryManagerWindowViewModel);
        }
    }
}
