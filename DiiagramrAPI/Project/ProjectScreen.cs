using DiiagramrAPI.Application;
using System;

namespace DiiagramrAPI.Project
{
    public class ProjectScreen : ViewModel
    {
        public ProjectScreen(
            Func<ProjectExplorer> projectExplorerViewModelFactory,
            Func<DiagramWell> diagramWellViewModelFactory)
        {
            ProjectExplorerViewModel = projectExplorerViewModelFactory.Invoke();
            DiagramWellViewModel = diagramWellViewModelFactory.Invoke();
        }

        public DiagramWell DiagramWellViewModel { get; set; }

        public ProjectExplorer ProjectExplorerViewModel { get; set; }
    }
}