using DiiagramrAPI.Project;
using System;
using System.Windows.Input;

namespace DiiagramrAPI.Application.ShellCommands.FileCommands
{
    public class VisualDropCloseProjectCommand : ShellCommandBase, IToolbarCommand, IHotkeyCommand
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

        public string ParentName => "Project";

        public float Weight => 1.0f;

        public Key Hotkey => Key.W;

        public bool RequiresCtrlModifierKey => true;

        public bool RequiresShiftModifierKey => true;

        public bool RequiresAltModifierKey => false;

        protected override void ExecuteInternal(object parameter)
        {
            _projectManager.CloseProject(() => _screenHost.ShowScreen(_startScreen));
        }

        protected override bool CanExecuteInternal()
        {
            return _projectManager.Project is object;
        }
    }
}