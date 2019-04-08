using DiiagramrAPI.Service.Interfaces;
using DiiagramrAPI.Shell;
using System;

namespace DiiagramrAPI.Service.Commands.FileCommands
{
    public class SaveProjectCommand : ToolBarCommand
    {
        private readonly VisualDropStartScreenViewModel _visualDropStartScreenViewModel;
        private readonly IProjectManager _projectManager;

        public override string Name => "Save";
        public override string Parent => "Project";
        public override float Weight => .5f;

        public SaveProjectCommand(Func<VisualDropStartScreenViewModel> visualDropStartScreenViewModelFactory, Func<IProjectManager> projectManagerFactory)
        {
            _visualDropStartScreenViewModel = visualDropStartScreenViewModelFactory.Invoke();
            _projectManager = projectManagerFactory.Invoke();
        }

        internal override void ExecuteInternal(IShell shell, object parameter)
        {
            _projectManager.SaveProject();
            shell.SetWindowTitle("Diiagramr" + (_projectManager.CurrentProject != null ? " - " + _projectManager.CurrentProject.Name : ""));
            shell.ShowScreen(_visualDropStartScreenViewModel);
        }
    }
}
