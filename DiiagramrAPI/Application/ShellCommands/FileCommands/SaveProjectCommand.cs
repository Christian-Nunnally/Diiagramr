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

        protected override void ExecuteInternal(object parameter)
        {
            _projectManager.SaveProject();
        }

        protected override bool CanExecuteInternal()
        {
            return _projectManager.CurrentProject is object;
        }
    }
}