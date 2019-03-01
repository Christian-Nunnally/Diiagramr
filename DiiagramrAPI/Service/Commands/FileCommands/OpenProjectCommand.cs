using DiiagramrAPI.Service.Interfaces;
using DiiagramrAPI.ViewModel;
using System;
using System.Linq;

namespace DiiagramrAPI.Service.Commands.FileCommands
{
    public class OpenProjectCommand : ToolBarCommand
    {
        private IProjectFileService _projectFileService;

        public override string Name => "Open";
        public override string Parent => "Project";
        public override float Weight => .9f;

        public OpenProjectCommand(Func<IProjectFileService> projectFileServiceFactory)
        {
            _projectFileService = projectFileServiceFactory.Invoke();
        }

        internal override void ExecuteInternal(ShellViewModel shell, object parameter)
        {
            var project = _projectFileService.LoadProject();
            shell.ProjectManager.LoadProject(project);
            var firstDiagram = shell.ProjectManager.CurrentDiagrams.FirstOrDefault();
            if (firstDiagram != null)
            {
                firstDiagram.Open();
            }
        }
    }
}
