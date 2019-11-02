using DiiagramrAPI.Service.Interfaces;
using System;

namespace DiiagramrAPI.Shell.ShellCommands.FileCommands
{
    public class SaveAsProjectCommand : ToolBarCommand
    {
        private readonly IProjectManager _projectManager;
        private readonly VisualDropStartScreenViewModel _visualDropStartScreenViewModel;

        public SaveAsProjectCommand(Func<VisualDropStartScreenViewModel> visualDropStartScreenViewModelFactory, Func<IProjectManager> projectManagerFactory)
        {
            _visualDropStartScreenViewModel = visualDropStartScreenViewModelFactory.Invoke();
            _projectManager = projectManagerFactory.Invoke();
        }

        public override string Name => "Save As...";
        public override string Parent => "Project";
        public override float Weight => .4f;

        internal override void ExecuteInternal(IShell shell, object parameter)
        {
            _projectManager.SaveAsProject();
            shell.SetWindowTitle("Visual Drop" + (_projectManager.CurrentProject != null ? " - " + _projectManager.CurrentProject.Name : ""));
            shell.ShowScreen(_visualDropStartScreenViewModel);
        }
    }
}