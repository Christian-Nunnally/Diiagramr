using DiiagramrAPI.Project;
using System;

namespace DiiagramrAPI.Shell.ShellCommands.FileCommands
{
    internal class CloseProjectCommand : DiiagramrCommand
    {
        private readonly ProjectManager _projectManager;
        private readonly StartScreenViewModel _startScreenViewModel;

        public CloseProjectCommand(Func<StartScreenViewModel> startScreenViewModelFactory, Func<ProjectManager> projectManagerFactory)
        {
            _startScreenViewModel = startScreenViewModelFactory.Invoke();
            _projectManager = projectManagerFactory.Invoke();
        }

        public override string Name => "Close";
        public override string Parent => "Project";

        internal override void ExecuteInternal(IShell shell, object parameter)
        {
            if (_projectManager.CloseProject())
            {
                shell.ShowScreen(_startScreenViewModel);
            }
        }
    }
}