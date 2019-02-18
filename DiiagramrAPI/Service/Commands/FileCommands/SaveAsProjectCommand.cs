using DiiagramrAPI.ViewModel;
using DiiagramrAPI.ViewModel.VisualDrop;
using System;

namespace DiiagramrAPI.Service.Commands.FileCommands
{
    public class SaveAsProjectCommand : ToolBarCommand
    {
        public override string Name => "Save As...";
        public override string Parent => "Project";
        public override float Weight => .4f;

        private readonly VisualDropStartScreenViewModel _visualDropStartScreenViewModel;

        public SaveAsProjectCommand(Func<VisualDropStartScreenViewModel> visualDropStartScreenViewModelFactory)
        {
            _visualDropStartScreenViewModel = visualDropStartScreenViewModelFactory.Invoke();
        }

        internal override void ExecuteInternal(ShellViewModel shell, object parameter)
        {
            shell.ProjectManager.SaveAsProject();
            shell.WindowTitle = "Diiagramr" + (shell.ProjectManager.CurrentProject != null ? " - " + shell.ProjectManager.CurrentProject.Name : "");
        }
    }
}
