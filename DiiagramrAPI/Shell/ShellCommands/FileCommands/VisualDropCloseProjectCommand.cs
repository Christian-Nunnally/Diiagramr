using DiiagramrAPI.Project;
using System;

namespace DiiagramrAPI.Shell.ShellCommands.FileCommands
{
    public class VisualDropCloseProjectCommand : ToolBarCommand
    {
        private readonly IProjectManager _projectManager;
        private readonly VisualDropStartScreenViewModel _startScreenViewModel;

        public VisualDropCloseProjectCommand(Func<VisualDropStartScreenViewModel> startScreenViewModelFactory, Func<IProjectManager> projectManagerFactory)
        {
            _startScreenViewModel = startScreenViewModelFactory.Invoke();
            _projectManager = projectManagerFactory.Invoke();
        }

        public override string Name => "Close";
        public override string Parent => "Project";
        public override float Weight => 0.1f;

        internal override void ExecuteInternal(IShell shell, object parameter)
        {
            if (_projectManager.CloseProject())
            {
                shell.ShowScreen(_startScreenViewModel);
            }
        }
    }
}