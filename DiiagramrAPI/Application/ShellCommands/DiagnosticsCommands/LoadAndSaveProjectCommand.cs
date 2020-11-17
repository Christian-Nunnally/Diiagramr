using DiiagramrAPI.Project;
using System;
using System.IO;

namespace DiiagramrAPI.Application.ShellCommands.DiagnosticsCommands
{
    /// <summary>
    /// Debugging only command to trst loading and saving a project.
    /// </summary>
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

        /// <inheritdoc/>
        public override string Name => "Load and Save Project";

        /// <inheritdoc/>
        public string ParentName => "Diagnostics";

        /// <inheritdoc/>
        public float Weight => 1f;

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        protected override bool CanExecuteInternal()
        {
            return _projectManager.Project is object;
        }
    }
}