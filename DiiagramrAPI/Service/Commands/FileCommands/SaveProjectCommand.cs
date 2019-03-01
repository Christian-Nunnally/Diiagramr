using DiiagramrAPI.ViewModel;

namespace DiiagramrAPI.Service.Commands.FileCommands
{
    public class SaveProjectCommand : ToolBarCommand
    {
        public override string Name => "Save";
        public override string Parent => "Project";
        public override float Weight => .5f;

        internal override void ExecuteInternal(ShellViewModel shell, object parameter)
        {
            shell.ProjectManager.SaveProject();
            shell.WindowTitle = "Diiagramr" + (shell.ProjectManager.CurrentProject != null ? " - " + shell.ProjectManager.CurrentProject.Name : "");
        }
    }
}
