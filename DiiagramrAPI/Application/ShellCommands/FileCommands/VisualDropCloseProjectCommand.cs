using DiiagramrAPI.Project;
using System;

namespace DiiagramrAPI.Application.ShellCommands.FileCommands
{
    public class VisualDropCloseProjectCommand : ToolBarCommand
    {
        private readonly IProjectManager _projectManager;
        private readonly VisualDropStartScreen _startScreen;
        private readonly ScreenHost _screenHost;

        public VisualDropCloseProjectCommand(
            Func<VisualDropStartScreen> startScreenFactory,
            Func<IProjectManager> projectManagerFactory,
            Func<ScreenHost> screenHostFactory)
        {
            _startScreen = startScreenFactory();
            _projectManager = projectManagerFactory();
            _screenHost = screenHostFactory();
        }

        public override string Name => "Close";

        public override string Parent => "Project";

        public override float Weight => 0.1f;

        protected override void ExecuteInternal(object parameter)
        {
            if (_projectManager.CloseProject())
            {
                _screenHost.ShowScreen(_startScreen);
            }
        }

        protected override bool CanExecuteInternal()
        {
            return _projectManager.CurrentProject is object;
        }
    }
}