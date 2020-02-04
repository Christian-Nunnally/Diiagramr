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

        public override float Weight => .5f;

        protected override void ExecuteInternal(object parameter)
        {
            _projectManager.SaveAsProject();
        }

        protected override bool CanExecuteInternal()
        {
            return _projectManager.Project is object;
        }
    }
}