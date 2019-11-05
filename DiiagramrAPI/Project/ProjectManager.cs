using DiiagramrAPI.Application.Tools;
using DiiagramrAPI.Editor.Diagrams;
using DiiagramrAPI.Service.Plugins;
using DiiagramrCore;
using DiiagramrModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace DiiagramrAPI.Project
{
    public class ProjectManager : IProjectManager
    {
        private readonly DiagramFactory _diagramViewModelFactory;
        private readonly ILibraryManager _libraryManager;
        private readonly IProjectFileService _projectFileService;

        public ProjectManager(
            Func<IProjectFileService> projectFileServiceFactory,
            Func<ILibraryManager> libraryManagerFactory,
            Func<DiagramFactory> diagramViewModelFactoryFactory)
        {
            Diagrams = new List<Diagram>();
            _libraryManager = libraryManagerFactory.Invoke();
            _projectFileService = projectFileServiceFactory.Invoke();
            _diagramViewModelFactory = diagramViewModelFactoryFactory.Invoke();
            CurrentProjectChanged += OnCurrentProjectChanged;
        }

        public event Action CurrentProjectChanged;

        public ObservableCollection<DiagramModel> CurrentDiagrams => CurrentProject?.Diagrams;

        public ProjectModel CurrentProject { get; set; }

        public IList<Diagram> Diagrams { get; }

        public bool IsProjectDirty => CurrentProject?.IsDirty ?? false;

        public bool CloseProject()
        {
            if (IsProjectDirty)
            {
                var result = _projectFileService.ConfirmProjectClose();
                if (result == MessageBoxResult.Cancel)
                {
                    return false;
                }

                if (result == MessageBoxResult.Yes)
                {
                    _projectFileService.SaveProject(CurrentProject, false);
                }
                else if (result == MessageBoxResult.No)
                {
                    CurrentProject.IsDirty = false;
                }
            }

            return true;
        }

        public void CreateDiagram()
        {
            CreateDiagram(new DiagramModel());
            if (CurrentDiagrams.Count == 1 && CurrentDiagrams.First().Nodes.Count == 0)
            {
                CurrentProject.IsDirty = false;
            }
        }

        public void CreateDiagram(DiagramModel diagram)
        {
            if (CurrentProject == null)
            {
                throw new NullReferenceException("ProjectModel does not exist");
            }

            if (diagram == null)
            {
                return;
            }

            var diagramName = string.IsNullOrEmpty(diagram.Name) ? "diagram" : diagram.Name;
            var diagramNumber = 1;
            while (CurrentProject.Diagrams.Any(x => x.Name.Equals(diagramName + diagramNumber)))
            {
                diagramNumber++;
            }

            diagram.Name = diagramName + diagramNumber;
            CreateDiagramViewModel(diagram);
            CurrentProject.AddDiagram(diagram);
        }

        public void CreateProject()
        {
            if (CloseProject())
            {
                CurrentProject = new ProjectModel();
                CurrentProjectChanged?.Invoke();
                CurrentProject.IsDirty = false;
            }
        }

        public void DeleteDiagram(DiagramModel diagram)
        {
            CurrentProject.RemoveDiagram(diagram);
            var diagramViewModel = Diagrams.FirstOrDefault(m => m.Model == diagram);
            if (diagramViewModel != null)
            {
                Diagrams.Remove(diagramViewModel);
            }
        }

        public IEnumerable<Type> GetSerializeableTypes()
        {
            return _libraryManager.GetSerializeableTypes();
        }

        public void LoadProject(ProjectModel project, bool autoOpenDiagram = false)
        {
            if (CloseProject())
            {
                CurrentProject = project;
                if (CurrentProject == null)
                {
                    return;
                }

                try
                {
                    CurrentProjectChanged?.Invoke();
                    CurrentProject.IsDirty = false;
                    if (autoOpenDiagram && CurrentDiagrams.Any())
                    {
                        CurrentDiagrams.First().IsOpen = true;
                    }
                    else if (autoOpenDiagram)
                    {
                        CreateDiagram();
                    }
                }
                catch (Exception)
                {
                    // This code is so that if a project fails to load because it contains a node from an uninstalled library,
                    // it will attempt to download and install the missing library. I'm pretty sure this code work as intended
                    // because nodes that fail to load are handled by printing a message to the output and just ignoring that node.
                    // TODO: Make async
                    DownloadProjectDependencies().Wait();
                    CurrentProjectChanged?.Invoke();
                    CurrentProject.IsDirty = false;
                    if (autoOpenDiagram && CurrentDiagrams.Any())
                    {
                        CurrentDiagrams.First().IsOpen = true;
                    }

                    // TODO: Catch specific types of exceptions.
                    throw;
                }
            }
        }

        public void SaveAsProject()
        {
            if (_projectFileService.SaveProject(CurrentProject, true))
            {
                CurrentProject.IsDirty = false;
            }
        }

        public void SaveProject()
        {
            if (_projectFileService.SaveProject(CurrentProject, false))
            {
                CurrentProject.IsDirty = false;
            }
        }

        private void CreateDiagramViewModel(DiagramModel diagram)
        {
            var diagramViewModel = _diagramViewModelFactory.CreateDiagramViewModel(diagram);
            Diagrams.Add(diagramViewModel);
        }

        private async Task DownloadProjectDependencies()
        {
            foreach (var diagram in CurrentProject.Diagrams)
            {
                foreach (var node in diagram.Nodes)
                {
                    if (node.Dependency != null)
                    {
                        await _libraryManager.InstallLatestVersionOfLibraryAsync(new LibraryListItem(node.Dependency));
                    }
                }
            }
        }

        private void OnCurrentProjectChanged()
        {
            Diagrams.Clear();
            CurrentDiagrams?.ForEach(CreateDiagramViewModel);
        }
    }
}