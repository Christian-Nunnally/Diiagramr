using DiiagramrAPI.Project;
using System;
using System.Windows.Input;

namespace DiiagramrAPI.Application.ShellCommands.FileCommands
{
    public class SaveProjectCommand : ShellCommandBase, IToolbarCommand, IHotkeyCommand
    {
        private readonly IProjectManager _projectManager;
        private readonly IProjectFileService _projectFileService;

        public SaveProjectCommand(
            Func<IProjectManager> projectManagerFactory,
            Func<IProjectFileService> projectFileServiceFactory)
        {
            _projectManager = projectManagerFactory();
            _projectFileService = projectFileServiceFactory();
        }

        public override string Name => "Save";

        public string ParentName => "Project";

        public float Weight => .4f;

        public Key Hotkey => Key.S;

        public bool RequiresCtrlModifierKey => true;

        public bool RequiresShiftModifierKey => false;

        public bool RequiresAltModifierKey => false;

        protected override void ExecuteInternal(object parameter)
        {
            if (_projectManager.Project != null)
            {
                _projectFileService.SaveProject(_projectManager.Project, saveAs: false, () => { });
            }
        }

        protected override bool CanExecuteInternal()
        {
            return _projectManager.Project is object;
        }
    }
}