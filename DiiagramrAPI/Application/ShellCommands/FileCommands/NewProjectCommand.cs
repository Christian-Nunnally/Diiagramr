using DiiagramrAPI.Project;
using System;
using System.Linq;

namespace DiiagramrAPI.Application.ShellCommands.FileCommands
{
    public class NewProjectCommand : ToolBarCommand
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

        public override string Parent => "Project";

        public override float Weight => 0.0f;

        protected override void ExecuteInternal(object parameter)
        {
            _projectManager.CreateProject(() =>
            {
                _projectManager.CreateDiagram();
                _diagramWell.OpenDiagram(_projectManager.Diagrams.First());
                _screenHost.ShowScreen(_projectScreen);
            });
        }
    }
}