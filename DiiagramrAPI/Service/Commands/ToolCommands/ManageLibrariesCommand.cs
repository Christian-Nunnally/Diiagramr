using DiiagramrAPI.ViewModel;
using System;

namespace DiiagramrAPI.Service.Commands.ToolCommands
{
    public class ManageLibrariesCommand : DiiagramrCommand
    {
        private readonly LibraryManagerWindowViewModel _libraryManagerWindowViewModel;

        public ManageLibrariesCommand(Func<LibraryManagerWindowViewModel> startScreenViewModelFactory)
        {
            _libraryManagerWindowViewModel = startScreenViewModelFactory.Invoke();
        }

        public override string Name => "Libraries";
        public override string Parent => "Tools";
        public override float Weight => .5f;

        public override void Execute(ShellViewModel shell)
        {
            shell.OpenWindow(_libraryManagerWindowViewModel);
        }
    }
}
