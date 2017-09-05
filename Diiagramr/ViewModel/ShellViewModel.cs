using System;
using DiagramEditor.Service;
using Stylet;

namespace Diiagramr.ViewModel
{
    public class ShellViewModel : Screen, IRequestClose
    {
        private readonly IProjectManager _projectManager;

        public ProjectExplorerViewModel ProjectExplorerViewModel { get; set; }

        public DiagramWellViewModel DiagramWellViewModel { get; set; }

        public ShellViewModel(Func<ProjectExplorerViewModel> projectExplorerViewModelFactory, Func<DiagramWellViewModel> diagramWellViewModelFactory, Func<IProjectManager> projectManagerFactory)
        {
            DiagramWellViewModel = diagramWellViewModelFactory.Invoke();
            ProjectExplorerViewModel = projectExplorerViewModelFactory.Invoke();
            _projectManager = projectManagerFactory.Invoke();

        }

        public override void RequestClose(bool? dialogResult = null)
        {
            _projectManager.CloseProject();
            if (Parent != null) base.RequestClose(dialogResult);
        }

        public void CreateProject()
        {
            _projectManager.CreateNewProject();
        }

        public void LoadProject(string projectPath)
        {
            _projectManager.LoadProject(projectPath);
        }

        public void SaveProject()
        {
            _projectManager.SaveProject();
        }

        public void Close()
        {
            RequestClose();
        }

        public void SaveAndClose()
        {
            _projectManager.SaveProject();
            RequestClose();
        }
    }
}