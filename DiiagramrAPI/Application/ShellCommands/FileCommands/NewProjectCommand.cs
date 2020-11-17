using DiiagramrAPI.Project;
using DiiagramrModel;
using System;
using System.Linq;
using System.Windows.Input;

namespace DiiagramrAPI.Application.ShellCommands.FileCommands
{
    /// <summary>
    /// Command that creates a new project.
    /// </summary>
    public class NewProjectCommand : ShellCommandBase, IToolbarCommand, IHotkeyCommand
    {
        private readonly IProjectManager _projectManager;
        private readonly ProjectScreen _projectScreen;
        private readonly ScreenHostBase _screenHost;
        private readonly DiagramWell _diagramWell;

        /// <summary>
        /// Creates a new instance of <see cref="NewProjectCommand"/>
        /// </summary>
        /// <param name="projectManagerFactory">Factory to get an instance of an <see cref="IProjectManager"/>.</param>
        /// <param name="projectScreenFactory">Factory to get an instance of an <see cref="ProjectScreen"/>.</param>
        /// <param name="screenHostFactory">Factory to get an instance of an <see cref="ScreenHostBase"/>.</param>
        /// <param name="diagramWellFactory">Factory to get an instance of an <see cref="DiagramWell"/>.</param>
        public NewProjectCommand(
            Func<IProjectManager> projectManagerFactory,
            Func<ProjectScreen> projectScreenFactory,
            Func<ScreenHostBase> screenHostFactory,
            Func<DiagramWell> diagramWellFactory)
        {
            _projectManager = projectManagerFactory();
            _projectScreen = projectScreenFactory();
            _screenHost = screenHostFactory();
            _diagramWell = diagramWellFactory();
        }

        /// <inheritdoc/>
        public override string Name => "New";

        /// <inheritdoc/>
        public string ParentName => "Project";

        /// <inheritdoc/>
        public float Weight => 0.0f;

        /// <inheritdoc/>
        public Key Hotkey => Key.N;

        /// <inheritdoc/>
        public bool RequiresCtrlModifierKey => true;

        /// <inheritdoc/>
        public bool RequiresShiftModifierKey => false;

        /// <inheritdoc/>
        public bool RequiresAltModifierKey => false;

        /// <inheritdoc/>
        protected override bool CanExecuteInternal()
        {
            return true;
        }

        /// <inheritdoc/>
        protected override void ExecuteInternal(object parameter)
        {
            _projectManager.CreateProject(() =>
            {
                _projectManager.InsertDiagram(new DiagramModel());
                _diagramWell.OpenDiagram(_projectManager.Diagrams.First());
                _screenHost.ShowScreen(_projectScreen);
            });
        }
    }
}