using DiiagramrAPI.Project;
using System;

namespace DiiagramrAPI.Application.ShellCommands.FileCommands
{
    public class SaveProjectCommand : ToolBarCommand
    {
        private readonly IProjectManager _projectManager;

        public SaveProjectCommand(Func<IProjectManager> projectManagerFactory)
        {
            _projectManager = projectManagerFactory();
        }

        public override string Name => "Save";

        public override string Parent => "Project";

        public override float Weight => .5f;

        internal override void ExecuteInternal(IApplicationShell shell, object parameter)
        {
            _projectManager.SaveProject();
            shell.SetWindowTitle("Diiagramr" + (_projectManager.CurrentProject != null ? " - " + _projectManager.CurrentProject.Name : string.Empty));
        }
    }
}