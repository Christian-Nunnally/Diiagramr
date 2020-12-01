using DiiagramrAPI.Project;
using DiiagramrModel;
using System;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace DiiagramrAPI.Application.ShellCommands.FileCommands
{
    /// <summary>
    /// Opens an existing project.
    /// </summary>
    public class
        OpenTemplateCommand : ShellCommandBase, IToolbarCommand, IHotkeyCommand
    {
        private readonly IProjectFileService _projectFileService;
        private readonly IProjectManager _projectManager;
        private readonly ProjectScreen _projectScreen;
        private readonly ScreenHostBase _screenHost;

        /// <summary>
        /// Creates a new instance of <see cref="OpenProjectCommand"/>
        /// </summary>
        /// <param name="projectFileServiceFactory">Factory to get an instance of an <see cref="IProjectFileService"/>.</param>
        /// <param name="projectManagerFactory">Factory to get an instance of an <see cref="IProjectManager"/>.</param>
        /// <param name="projectScreenFactory">Factory to get an instance of an <see cref="ProjectScreen"/>.</param>
        /// <param name="screenHostFactory">Factory to get an instance of an <see cref="ScreenHostBase"/>.</param>
        public OpenTemplateCommand(
            Func<IProjectFileService> projectFileServiceFactory,
            Func<IProjectManager> projectManagerFactory,
            Func<ProjectScreen> projectScreenFactory,
            Func<ScreenHostBase> screenHostFactory)
        {
            _projectFileService = projectFileServiceFactory();
            _projectFileService.DirectoryToService = ProjectFileService.TemplatesSaveDirectoryPath;
            _projectManager = projectManagerFactory();
            _projectScreen = projectScreenFactory();
            _screenHost = screenHostFactory();
        }

        /// <inheritdoc/>
        public override string Name => "Open template";

        /// <inheritdoc/>
        public string ParentName => "Project";

        /// <inheritdoc/>
        public float Weight => .11f;

        /// <inheritdoc/>
        public Key Hotkey => Key.T;

        /// <inheritdoc/>
        public bool RequiresCtrlModifierKey => true;

        /// <inheritdoc/>
        public bool RequiresAltModifierKey => false;

        /// <inheritdoc/>
        public bool RequiresShiftModifierKey => false;

        /// <inheritdoc/>
        protected override bool CanExecuteInternal()
        {
            return true;
        }

        /// <inheritdoc/>
        protected override void ExecuteInternal(object parameter)
        {
            if (parameter is string projectName)
            {
                projectName += projectName.EndsWith(ProjectFileService.ProjectFileExtension) ? string.Empty : ProjectFileService.ProjectFileExtension;
                var projectPath = Path.Combine(ProjectFileService.TemplatesSaveDirectoryPath, projectName).Replace(@"\\", @"\");
                var project = _projectFileService.LoadProject(projectPath);
                if (project is object) ProjectLoaded(project);
            }
            else
            {
                _projectFileService.LoadProject(ProjectLoaded);
            }
        }

        private void ProjectLoaded(ProjectModel project)
        {
            project.Name += "Copy";
            _projectManager.SetProject(project);
            _projectScreen.OpenDiagram(_projectManager.Diagrams?.FirstOrDefault());
            _screenHost.ShowScreen(_projectScreen);
        }
    }
}