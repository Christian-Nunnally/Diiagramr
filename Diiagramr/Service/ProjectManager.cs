using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using Diiagramr.Model;
using Diiagramr.Service.Interfaces;
using Diiagramr.ViewModel.Diagram;

namespace Diiagramr.Service
{
    public class ProjectManager : IProjectManager
    {
        private readonly IProvideNodes _nodeProvider;
        private readonly IProjectFileService _projectFileService;
        public IList<DiagramViewModel> DiagramViewModels { get; }

        public ProjectManager(Func<IProjectFileService> projectFileServiceFactory, Func<IProvideNodes> nodeProviderFactory)
        {
            DiagramViewModels = new List<DiagramViewModel>();
            _projectFileService = projectFileServiceFactory.Invoke();
            _nodeProvider = nodeProviderFactory.Invoke();
            _nodeProvider.ProjectManager = this;
            CurrentProjectChanged += OnCurrentProjectChanged;
        }

        public event Action CurrentProjectChanged;
        public ProjectModel CurrentProject { get; set; }
        public bool IsProjectDirty { get; set; }
        public ObservableCollection<DiagramModel> CurrentDiagrams => CurrentProject?.Diagrams;

        public void CreateProject()
        {
            if (CloseProject())
            {
                CurrentProject = new ProjectModel();
                IsProjectDirty = true;
                CurrentProjectChanged?.Invoke();
            }
        }

        public void SaveProject()
        {
            if (_projectFileService.SaveProject(CurrentProject, false))
                IsProjectDirty = false;
        }

        public void SaveAsProject()
        {
            if (_projectFileService.SaveProject(CurrentProject, true))
                IsProjectDirty = false;
        }

        public void LoadProject()
        {
            if (CloseProject())
            {
                CurrentProject = _projectFileService.LoadProject();
                IsProjectDirty = false;
                CurrentProjectChanged?.Invoke();
            }
        }

        public bool CloseProject()
        {
            if (IsProjectDirty)
            {
                var result = _projectFileService.ConfirmProjectClose();
                if (result == DialogResult.Cancel)
                    return false;
                if (result == DialogResult.Yes)
                    _projectFileService.SaveProject(CurrentProject, false);
            }
            return true;
        }

        public void CreateDiagram()
        {
            CreateDiagram(new DiagramModel());
        }

        public void CreateDiagram(DiagramModel diagramModel)
        {
            if (CurrentProject == null)
                throw new NullReferenceException("ProjectModel does not exist");
            string dName = string.IsNullOrEmpty(diagramModel.Name) ? "diagram" : diagramModel.Name;
            var dNum = 1;
            while (CurrentProject.Diagrams.Any(x => x.Name.Equals(dName + dNum)))
                dNum++;
            diagramModel.Name = dName + dNum;
            IsProjectDirty = true;
            CreateDiagramViewModel(diagramModel);
            CurrentProject.Diagrams.Add(diagramModel);
        }

        public void DeleteDiagram(DiagramModel diagram)
        {
            CurrentProject.Diagrams.Remove(diagram);
            DiagramViewModels.Remove(DiagramViewModels.First(m => m.Diagram == diagram));
            IsProjectDirty = true;
        }

        private void OnCurrentProjectChanged()
        {
            DiagramViewModels.Clear();
            CurrentDiagrams?.ForEach(CreateDiagramViewModel);
        }

        private void CreateDiagramViewModel(DiagramModel diagram)
        {
            var diagramViewModel = new DiagramViewModel(diagram, _nodeProvider);
            DiagramViewModels.Add(diagramViewModel);
        }
    }
}