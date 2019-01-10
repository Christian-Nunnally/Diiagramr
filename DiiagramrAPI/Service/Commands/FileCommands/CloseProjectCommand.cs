using DiiagramrAPI.ViewModel;
using System;

namespace DiiagramrAPI.Service.Commands.FileCommands
{
    public class CloseProjectCommand : DiiagramrCommand
    {
        private readonly StartScreenViewModel _startScreenViewModel;

        public override string Parent => "File";
        public override string Name => "Close";

        public CloseProjectCommand(Func<StartScreenViewModel> startScreenViewModelFactory)
        {
            _startScreenViewModel = startScreenViewModelFactory.Invoke();
        }

        public override void Execute(ShellViewModel shell)
        {
            if (shell.ProjectScreenViewModel.ProjectExplorerViewModel.ProjectManager.CloseProject())
            {
                shell.ShowScreen(_startScreenViewModel);
            }
        }
    }
}
