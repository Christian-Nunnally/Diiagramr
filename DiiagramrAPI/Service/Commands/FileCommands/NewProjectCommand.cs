using DiiagramrAPI.ViewModel;

namespace DiiagramrAPI.Service.Commands.FileCommands
{
    public class NewProjectCommand : DiiagramrCommand
    {
        public override string Parent => "File";
        public override string Name => "New";
        public override float Weight => 1.0f;

        public override void Execute(ShellViewModel shell)
        {
            shell.ProjectManager.CreateProject();
        }
    }
}
