using DiiagramrAPI.ViewModel;

namespace DiiagramrAPI.Service.Commands.FileCommands
{
    public class SaveAsProjectCommand : DiiagramrCommand
    {
        public override string Name => "Save As...";
        public override string Parent => "File";
        public override float Weight => .4f;

        public override void Execute(ShellViewModel shell)
        {
            shell.ProjectManager.SaveAsProject();
            shell.WindowTitle = "Diiagramr" + (shell.ProjectManager.CurrentProject != null ? " - " + shell.ProjectManager.CurrentProject.Name : "");
        }
    }
}
