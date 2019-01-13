using DiiagramrAPI.ViewModel;

namespace DiiagramrAPI.Service.Commands.FileCommands
{
    public class SaveProjectCommand : DiiagramrCommand
    {
        public override string Name => "Save";
        public override string Parent => "File";
        public override float Weight => .5f;

        public override void Execute(ShellViewModel shell)
        {
            shell.ProjectManager.SaveProject();
            shell.WindowTitle = "Diiagramr" + (shell.ProjectManager.CurrentProject != null ? " - " + shell.ProjectManager.CurrentProject.Name : "");
        }
    }
}
