using DiiagramrAPI.Service.Interfaces;
using DiiagramrAPI.Shell;
using System;

namespace DiiagramrAPI.Service.Commands.FileCommands
{
    public class SaveAsProjectCommand : ToolBarCommand
    {
        public override string Name => "Save As...";
        public override string Parent => "Project";
        public override float Weight => .4f;

        private readonly VisualDropStartScreenViewModel _visualDropStartScreenViewModel;
        private readonly IProjectManager _projectManager;

        public SaveAsProjectCommand(Func<VisualDropStartScreenViewModel> visualDropStartScreenViewModelFactory, Func<IProjectManager> projectManagerFactory)
        {
            _visualDropStartScreenViewModel = visualDropStartScreenViewModelFactory.Invoke();
            _projectManager = projectManagerFactory.Invoke();
        }

        internal override void ExecuteInternal(IShell shell, object parameter)
        {
            _projectManager.SaveAsProject();
            shell.SetWindowTitle("Visual Drop" + (_projectManager.CurrentProject != null ? " - " + _projectManager.CurrentProject.Name : ""));
            shell.ShowScreen(_visualDropStartScreenViewModel);
        }
    }
}
