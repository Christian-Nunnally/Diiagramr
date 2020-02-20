using DiiagramrAPI.Project;
using DiiagramrModel;
using System;
using System.Linq;
using System.Windows.Input;

namespace DiiagramrAPI.Application.ShellCommands.FileCommands
{
    public class NewProjectCommand : ShellCommandBase, IToolbarCommand, IHotkeyCommand
    {
        private readonly IProjectManager _projectManager;
        private readonly ProjectScreen _projectScreen;
        private readonly ScreenHost _screenHost;
        private readonly DiagramWell _diagramWell;

        public NewProjectCommand(
            Func<IProjectManager> projectManagerFactory,
            Func<ProjectScreen> projectScreenFactory,
            Func<ScreenHost> screenHostFactory,
            Func<DiagramWell> diagramWellFactory)
        {
            _projectManager = projectManagerFactory();
            _projectScreen = projectScreenFactory();
            _screenHost = screenHostFactory();
            _diagramWell = diagramWellFactory();
        }

        public override string Name => "New";

        public string ParentName => "Project";

        public float Weight => 0.0f;

        public Key Hotkey => Key.N;

        public bool RequiresCtrlModifierKey => true;

        public bool RequiresShiftModifierKey => false;

        public bool RequiresAltModifierKey => false;

        protected override bool CanExecuteInternal()
        {
            return true;
        }

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