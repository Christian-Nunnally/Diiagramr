using DiiagramrAPI.Diagram.Model;
using DiiagramrAPI.Project;
using DiiagramrAPI.Service;
using DiiagramrAPI.Service.Interfaces;
using System;
using System.IO;
using System.Linq;

namespace DiiagramrAPI.Shell.ShellCommands.FileCommands
{
    public class OpenProjectCommand : ToolBarCommand
    {
        private readonly IProjectFileService _projectFileService;
        private readonly IProjectManager _projectManager;
        private readonly ProjectScreen _projectScreen;

        public override string Name => "Open";
        public override string Parent => "Project";
        public override float Weight => .9f;

        public OpenProjectCommand(Func<IProjectFileService> projectFileServiceFactory, Func<IProjectManager> projectManagerFactory, Func<ProjectScreen> projectScreenViewModelFactory)
        {
            _projectFileService = projectFileServiceFactory.Invoke();
            _projectManager = projectManagerFactory.Invoke();
            _projectScreen = projectScreenViewModelFactory.Invoke();
        }

        internal override void ExecuteInternal(IShell shell, object parameter)
        {
            ProjectModel project;
            if (parameter is string projectName)
            {
                projectName += projectName.EndsWith(ProjectFileService.ProjectFileExtension) ? string.Empty : ProjectFileService.ProjectFileExtension;
                var projectPath = Path.Combine(_projectFileService.ProjectDirectory, projectName).Replace(@"\\", @"\");
                project = _projectFileService.LoadProject(projectPath);

            }
            else
            {
                project = _projectFileService.LoadProject();
            }
            if (project != null)
            {
                LoadProject(shell, project);
            }
        }

        private void LoadProject(IShell shell, ProjectModel project)
        {
            _projectManager.LoadProject(project);
            var firstDiagram = _projectManager?.CurrentDiagrams?.FirstOrDefault();
            if (firstDiagram != null)
            {
                firstDiagram.Open();
            }
            shell.ShowScreen(_projectScreen);
        }
    }
}
