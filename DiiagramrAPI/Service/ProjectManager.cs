using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using DiiagramrAPI.Model;
using DiiagramrAPI.Service.Interfaces;
using DiiagramrAPI.ViewModel.Diagram;
using DiiagramrAPI.ViewModel.ShellScreen;

namespace DiiagramrAPI.Service
{
    public class ProjectManager : IProjectManager
    {
        private readonly IProvideNodes _nodeProvider;
        private readonly IProjectFileService _projectFileService;
        private readonly LibraryManagerScreenViewModel _libraryManager;
        public IList<DiagramViewModel> DiagramViewModels { get; }

        public ProjectManager(Func<IProjectFileService> projectFileServiceFactory, Func<IProvideNodes> nodeProviderFactory, Func<LibraryManagerScreenViewModel> libraryManager)
        {
            DiagramViewModels = new List<DiagramViewModel>();
            _libraryManager = libraryManager.Invoke();
            _projectFileService = projectFileServiceFactory.Invoke();
            _nodeProvider = nodeProviderFactory.Invoke();
            _nodeProvider.ProjectManager = this;
            CurrentProjectChanged += OnCurrentProjectChanged;
        }

        public event Action CurrentProjectChanged;
        public ProjectModel CurrentProject { get; set; }
        public bool IsProjectDirty => CurrentProject?.IsDirty ?? false;
        public ObservableCollection<DiagramModel> CurrentDiagrams => CurrentProject?.Diagrams;

        public void CreateProject()
        {
            if (CloseProject())
            {
                CurrentProject = new ProjectModel();
                CurrentProjectChanged?.Invoke();
            }
        }

        public void SaveProject()
        {
            if (_projectFileService.SaveProject(CurrentProject, false))
                CurrentProject.IsDirty = false;
        }

        public void SaveAsProject()
        {
            if (_projectFileService.SaveProject(CurrentProject, true))
                CurrentProject.IsDirty = false;
        }

        public void LoadProject()
        {
            if (CloseProject())
            {
                CurrentProject = _projectFileService.LoadProject();
                DownloadProjectDependencies();
                CurrentProject.IsDirty = false;
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

        public void CreateDiagram(DiagramModel diagram)
        {
            if (CurrentProject == null) throw new NullReferenceException("ProjectModel does not exist");
            var diagramName = string.IsNullOrEmpty(diagram.Name) ? "diagram" : diagram.Name;
            var diagramNumber = 1;
            while (CurrentProject.Diagrams.Any(x => x.Name.Equals(diagramName + diagramNumber))) diagramNumber++;
            diagram.Name = diagramName + diagramNumber;
            CreateDiagramViewModel(diagram);
            CurrentProject.AddDiagram(diagram);
        }

        public void DeleteDiagram(DiagramModel diagram)
        {
            CurrentProject.RemoveDiagram(diagram);
            var diagramViewModel = DiagramViewModels.FirstOrDefault(m => m.Diagram == diagram);
            if (diagramViewModel != null) DiagramViewModels.Remove(diagramViewModel);
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

        private void DownloadProjectDependencies()
        {
            if (CurrentProject == null) return;
            foreach (var diagram in CurrentProject.Diagrams)
                foreach (var node in diagram.Nodes)
                    _libraryManager.InstallLibrary(node.Dependency.LibraryName, node.Dependency.LibraryVersion);
        }
    }
}