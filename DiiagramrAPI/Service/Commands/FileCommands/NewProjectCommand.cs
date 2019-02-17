using DiiagramrAPI.ViewModel;
using System.Linq;

namespace DiiagramrAPI.Service.Commands.FileCommands
{
    public class NewProjectCommand : ToolBarCommand
    {
        public override string Name => "New";
        public override string Parent => "Project";
        public override float Weight => 1.0f;

        internal override void ExecuteInternal(ShellViewModel shell, object parameter)
        {
            shell.ProjectManager.CreateProject();
            shell.ProjectManager.CreateDiagram();
            shell.ProjectManager.CurrentDiagrams.First().IsOpen = true;
            shell.ShowScreen(shell.ProjectScreenViewModel);
        }
    }
}
