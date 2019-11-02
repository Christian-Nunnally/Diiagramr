using DiiagramrAPI.Project;
using System;
using System.Linq;

namespace DiiagramrAPI.Application.ShellCommands.FileCommands
{
    public class NewProjectCommand : ToolBarCommand
    {
        private readonly IProjectManager _projectManager;
        private readonly ProjectScreen _projectScreenViewModel;

        public NewProjectCommand(Func<ProjectScreen> projectScreenViewModelFactory, Func<IProjectManager> projectManagerFactory)
        {
            _projectManager = projectManagerFactory.Invoke();
            _projectScreenViewModel = projectScreenViewModelFactory.Invoke();
        }

        public override string Name => "New";
        public override string Parent => "Project";
        public override float Weight => 1.0f;

        internal override void ExecuteInternal(IApplicationShell shell, object parameter)
        {
            _projectManager.CreateProject();
            _projectManager.CreateDiagram();
            _projectManager.CurrentDiagrams.First().IsOpen = true;
            shell.ShowScreen(_projectScreenViewModel);
        }
    }
}