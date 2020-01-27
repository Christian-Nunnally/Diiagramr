using DiiagramrAPI.Project;
using System;

namespace DiiagramrAPI.Application.ShellCommands.FileCommands
{
    public class CloseProjectCommand : ShellCommandBase
    {
        private readonly ProjectManager _projectManager;
        private readonly ScreenHost _screenHost;
        private readonly StartScreen _startScreen;

        public CloseProjectCommand(
            Func<StartScreen> startScreenFactory,
            Func<ProjectManager> projectManagerFactory,
            Func<ScreenHost> screenHostFactory)
        {
            _startScreen = startScreenFactory();
            _projectManager = projectManagerFactory();
            _screenHost = screenHostFactory();
        }

        public override string Name => "Close";

        public override string Parent => "Project";

        protected override bool CanExecuteInternal()
        {
            return _projectManager.CurrentProject is object;
        }

        protected override void ExecuteInternal(object parameter)
        {
            if (_projectManager.CloseProject())
            {
                _screenHost.ShowScreen(_startScreen);
            }
        }
    }
}