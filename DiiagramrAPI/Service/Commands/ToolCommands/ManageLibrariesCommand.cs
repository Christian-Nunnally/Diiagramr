using DiiagramrAPI.ViewModel;
using System;

namespace DiiagramrAPI.Service.Commands.ToolCommands
{
    public class ManageLibrariesCommand : DiiagramrCommand
    {
        private readonly LibraryManagerWindowViewModel _libraryManagerWindowViewModel;

        public override string Parent => "Tools";
        public override string Name => "Libraries";
        public override float Weight => .5f;

        public ManageLibrariesCommand(Func<LibraryManagerWindowViewModel> startScreenViewModelFactory)
        {
            _libraryManagerWindowViewModel = startScreenViewModelFactory.Invoke();
        }

        public override void Execute(ShellViewModel shell)
        {
            shell.OpenWindow(_libraryManagerWindowViewModel);
        }
    }
}
