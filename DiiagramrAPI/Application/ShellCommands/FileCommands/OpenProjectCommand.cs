using DiiagramrAPI.Project;
using DiiagramrModel;
using System;
using System.IO;
using System.Linq;

namespace DiiagramrAPI.Application.ShellCommands.FileCommands
{
    public class
        OpenProjectCommand : ToolBarCommand
    {
        private readonly IProjectFileService _projectFileService;
        private readonly IProjectManager _projectManager;
        private readonly ProjectScreen _projectScreen;
        private readonly ScreenHost _screenHost;

        public OpenProjectCommand(
            Func<IProjectFileService> projectFileServiceFactory,
            Func<IProjectManager> projectManagerFactory,
            Func<ProjectScreen> projectScreenFactory,
            Func<ScreenHost> screenHostFactory)
        {
            _projectFileService = projectFileServiceFactory();
            _projectManager = projectManagerFactory();
            _projectScreen = projectScreenFactory();
            _screenHost = screenHostFactory();
        }

        public override string Name => "Open";

        public override string Parent => "Project";

        public override float Weight => .1f;

        protected override void ExecuteInternal(object parameter)
        {
            if (parameter is string projectName)
            {
                projectName += projectName.EndsWith(ProjectFileService.ProjectFileExtension) ? string.Empty : ProjectFileService.ProjectFileExtension;
                var projectPath = Path.Combine(_projectFileService.ProjectDirectory, projectName).Replace(@"\\", @"\");
                var project = _projectFileService.LoadProject(projectPath);
                LoadProject(project);
            }
            else
            {
                _projectFileService.LoadProject(LoadProject);
            }
        }

        private void LoadProject(ProjectModel project)
        {
            _projectManager.LoadProject(project);
            var firstDiagram = _projectManager?.CurrentDiagrams?.FirstOrDefault();
            if (firstDiagram != null)
            {
                firstDiagram.Open();
            }

            _screenHost.ShowScreen(_projectScreen);
        }
    }
}