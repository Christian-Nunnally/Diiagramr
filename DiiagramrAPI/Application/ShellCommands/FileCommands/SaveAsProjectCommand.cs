using DiiagramrAPI.Project;
using System;

namespace DiiagramrAPI.Application.ShellCommands.FileCommands
{
    public class SaveAsProjectCommand : ToolBarCommand
    {
        private readonly IProjectManager _projectManager;

        public SaveAsProjectCommand(Func<IProjectManager> projectManagerFactory)
        {
            _projectManager = projectManagerFactory();
        }

        public override string Name => "Save As...";

        public override string Parent => "Project";

        public override float Weight => .4f;

        internal override void ExecuteInternal(IApplicationShell shell, object parameter)
        {
            _projectManager.SaveAsProject();
            shell.SetWindowTitle("Visual Drop" + (_projectManager.CurrentProject != null ? " - " + _projectManager.CurrentProject.Name : string.Empty));
        }
    }
}