using DiiagramrAPI.Service.Interfaces;
using DiiagramrAPI.Shell;
using System;

namespace DiiagramrAPI.Project
{
    public class ProjectScreen : ViewModel
    {
        private readonly IProjectManager _projectManager;

        public ProjectScreen(
            Func<ProjectExplorer> projectExplorerViewModelFactory,
            Func<DiagramWellViewModel> diagramWellViewModelFactory,
            Func<IProjectManager> projectManagerFactory)
        {
            _projectManager = projectManagerFactory.Invoke();

            ProjectExplorerViewModel = projectExplorerViewModelFactory.Invoke();
            DiagramWellViewModel = diagramWellViewModelFactory.Invoke();
        }

        public DiagramWellViewModel DiagramWellViewModel { get; set; }
        public ProjectExplorer ProjectExplorerViewModel { get; set; }
    }
}
