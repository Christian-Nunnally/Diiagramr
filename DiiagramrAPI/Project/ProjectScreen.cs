using DiiagramrAPI.Application;
using System;

namespace DiiagramrAPI.Project
{
    public class ProjectScreen : ViewModel
    {
        private readonly IProjectManager _projectManager;

        public ProjectScreen(
            Func<ProjectExplorer> projectExplorerViewModelFactory,
            Func<DiagramWell> diagramWellViewModelFactory,
            Func<IProjectManager> projectManagerFactory)
        {
            _projectManager = projectManagerFactory.Invoke();

            ProjectExplorerViewModel = projectExplorerViewModelFactory.Invoke();
            DiagramWellViewModel = diagramWellViewModelFactory.Invoke();
        }

        public DiagramWell DiagramWellViewModel { get; set; }
        public ProjectExplorer ProjectExplorerViewModel { get; set; }
    }
}