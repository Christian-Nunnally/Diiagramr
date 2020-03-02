using DiiagramrAPI.Project;
using DiiagramrModel;
using System;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace DiiagramrAPI.Application.ShellCommands.FileCommands
{
    /// <summary>
    /// Opens a project.
    /// </summary>
    public class
        OpenProjectCommand : ShellCommandBase, IToolbarCommand, IHotkeyCommand
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

        public string ParentName => "Project";

        public float Weight => .1f;

        public Key Hotkey => Key.O;

        public bool RequiresCtrlModifierKey => true;

        public bool RequiresAltModifierKey => false;

        public bool RequiresShiftModifierKey => false;

        protected override bool CanExecuteInternal()
        {
            return true;
        }

        protected override void ExecuteInternal(object parameter)
        {
            if (parameter is string projectName)
            {
                projectName += projectName.EndsWith(ProjectFileService.ProjectFileExtension) ? string.Empty : ProjectFileService.ProjectFileExtension;
                var projectPath = Path.Combine(_projectFileService.ProjectDirectory, projectName).Replace(@"\\", @"\");
                var project = _projectFileService.LoadProject(projectPath);
                if (project is object) LoadProject(project);
            }
            else
            {
                _projectFileService.LoadProject(LoadProject);
            }
        }

        private void LoadProject(ProjectModel project)
        {
            _projectManager.SetProject(project);
            _projectScreen.OpenDiagram(_projectManager.Diagrams?.FirstOrDefault());
            _screenHost.ShowScreen(_projectScreen);
        }
    }
}