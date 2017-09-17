using Diiagramr.Service;
using Stylet;
using System;
using Diiagramr.Service.Interfaces;

namespace Diiagramr.ViewModel
{
    public class ShellViewModel : Screen, IRequestClose
    {
        private readonly IProjectManager _projectManager;

        public ProjectExplorerViewModel ProjectExplorerViewModel { get; set; }

        public DiagramWellViewModel DiagramWellViewModel { get; set; }

        public bool CanSaveProject { get; set; }

        public bool CanSaveAsProject { get; set; }

        public ShellViewModel(Func<ProjectExplorerViewModel> projectExplorerViewModelFactory, Func<DiagramWellViewModel> diagramWellViewModelFactory, Func<IProjectManager> projectManagerFactory)
        {
            DiagramWellViewModel = diagramWellViewModelFactory.Invoke();
            ProjectExplorerViewModel = projectExplorerViewModelFactory.Invoke();
            _projectManager = projectManagerFactory.Invoke();

            _projectManager.CurrentProjectChanged += ProjectManagerOnCurrentProjectChanged;

        }

        private void ProjectManagerOnCurrentProjectChanged()
        {
            CanSaveProject = _projectManager.CurrentProject != null;
            CanSaveAsProject = _projectManager.CurrentProject != null;
        }

        public override void RequestClose(bool? dialogResult = null)
        {
            if (_projectManager.CloseProject())
            {
                if (Parent != null) base.RequestClose(dialogResult);
            }
        }

        public void CreateProject()
        {
            _projectManager.CreateProject();
        }

        public void LoadProject()
        {
            _projectManager.LoadProject();
        }

        public void SaveProject()
        {
            _projectManager.SaveProject();
        }

        public void SaveAsProject()
        {
            _projectManager.SaveAsProject();
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