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

        public NewProjectCommand(
            Func<IProjectManager> projectManagerFactory,
            Func<ProjectScreen> projectScreenFactory,
            Func<ScreenHost> screenHostFactory)
        {
            _projectManager = projectManagerFactory();
            _projectScreen = projectScreenFactory();
            _screenHost = screenHostFactory();
        }

        public override string Name => "New";

        public override string Parent => "Project";

        public override float Weight => 0.0f;

        protected override void ExecuteInternal(object parameter)
        {
            _projectManager.CreateProject(() =>
            {
                _projectManager.CreateDiagram();
                _projectManager.CurrentDiagrams.First().IsOpen = true;
                _screenHost.ShowScreen(_projectScreen);
            });
        }
    }
}