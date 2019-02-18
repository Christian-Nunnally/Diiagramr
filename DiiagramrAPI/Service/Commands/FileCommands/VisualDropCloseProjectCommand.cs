using DiiagramrAPI.ViewModel;
using DiiagramrAPI.ViewModel.VisualDrop;
using System;

namespace DiiagramrAPI.Service.Commands.FileCommands
{
    public class VisualDropCloseProjectCommand : ToolBarCommand
    {
        private readonly VisualDropStartScreenViewModel _startScreenViewModel;

        public VisualDropCloseProjectCommand(Func<VisualDropStartScreenViewModel> startScreenViewModelFactory)
        {
            _startScreenViewModel = startScreenViewModelFactory.Invoke();
        }

        public override string Name => "Close";
        public override string Parent => "Project";
        public override float Weight => 0.1f;

        internal override void ExecuteInternal(ShellViewModel shell, object parameter)
        {
            if (shell.ProjectScreenViewModel.ProjectExplorerViewModel.ProjectManager.CloseProject())
            {
                shell.ShowScreen(_startScreenViewModel);
            }
        }
    }
}
