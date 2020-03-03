using DiiagramrAPI.Project;
using System;
using System.IO;

namespace DiiagramrAPI.Application.ShellCommands.DiagnosticsCommands
{
    public class LoadAndSaveProjectCommand : ShellCommandBase, IToolbarCommand
    {
        private readonly IProjectManager _projectManager;
        private readonly IProjectFileService _projectFileService;

        public LoadAndSaveProjectCommand(
            Func<IProjectManager> projectManagerFactory,
            Func<IProjectFileService> projectFileServiceFactory)
        {
            _projectManager = projectManagerFactory();
            _projectFileService = projectFileServiceFactory();
        }

        public override string Name => "Load and Save Project";

        public string ParentName => "Diagnostics";

        public float Weight => 1f;

        protected override void ExecuteInternal(object parameter)
        {
            var projectName = _projectManager.Project.Name;
            _projectFileService.SaveProject(_projectManager.Project, saveAs: false, () =>
            {
                _projectManager.Project = null;
                _projectManager.CloseProject(() =>
                {
                    projectName += projectName.EndsWith(ProjectFileService.ProjectFileExtension) ? string.Empty : ProjectFileService.ProjectFileExtension;
                    var projectPath = Path.Combine(_projectFileService.ProjectDirectory, projectName).Replace(@"\\", @"\");
                    var project = _projectFileService.LoadProject(projectPath);
                    _projectManager.SetProject(project, true);
                });
            });
        }

        protected override bool CanExecuteInternal()
        {
            return _projectManager.Project is object;
        }
    }
}