using DiiagramrAPI.Service.Interfaces;
using Stylet;
using System;

namespace DiiagramrAPI.ViewModel.ProjectScreen
{
    public class ProjectScreenViewModel : Screen
    {
        private readonly IProjectManager _projectManager;
        public ProjectExplorerViewModel ProjectExplorerViewModel { get; set; }
        public DiagramWellViewModel DiagramWellViewModel { get; set; }

        public ProjectScreenViewModel(
            Func<ProjectExplorerViewModel> projectExplorerViewModelFactory,
            Func<DiagramWellViewModel> diagramWellViewModelFactory,
            Func<IProjectManager> projectManagerFactory)
        {
            _projectManager = projectManagerFactory.Invoke();

            ProjectExplorerViewModel = projectExplorerViewModelFactory.Invoke();
            DiagramWellViewModel = diagramWellViewModelFactory.Invoke();
        }
    }
}
