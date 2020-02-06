using DiiagramrAPI.Application;
using DiiagramrAPI.Application.Tools;
using DiiagramrAPI.Editor.Diagrams;
using DiiagramrAPI.Service.Editor;
using DiiagramrAPI.Service.Plugins;
using DiiagramrCore;
using DiiagramrModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace DiiagramrAPI.Project
{
    public class ProjectManager : IProjectManager
    {
        private readonly DiagramFactory _diagramFactory;
        private readonly DialogHost _dialogHost;
        private readonly ILibraryManager _libraryManager;
        private readonly IProjectFileService _projectFileService;
        private readonly NodeServiceProvider _nodeServiceProvider;

        public ProjectManager(
            Func<DialogHost> dialogHostFactory,
            Func<DiagramFactory> diagramFactoryFactory,
            Func<ILibraryManager> libraryManagerFactory,
            Func<IProjectFileService> projectFileServiceFactory,
            Func<NodeServiceProvider> nodeServiceProviderFactory)
        {
            _dialogHost = dialogHostFactory();
            _libraryManager = libraryManagerFactory();
            _projectFileService = projectFileServiceFactory();
            _diagramFactory = diagramFactoryFactory();
            _nodeServiceProvider = nodeServiceProviderFactory();
            CurrentProjectChanged += OnCurrentProjectChanged;
            _nodeServiceProvider.RegisterService<IProjectManager>(this);
        }

        public event Action CurrentProjectChanged;

        public ObservableCollection<DiagramModel> CurrentDiagrams => Project?.Diagrams;

        public ProjectModel Project { get; set; }

        public ObservableCollection<Diagram> Diagrams { get; } = new ObservableCollection<Diagram>();

        public bool IsProjectDirty => Project?.IsDirty ?? false;

        public void CloseProject(Action continuation)
        {
            if (Project == null)
            {
                continuation();
                return;
            }

            void newContinuation() { Project = null; continuation(); }
            var saveBeforeCloseMessageBox = new Application.Dialogs.MessageBox.Builder("Close Project", "Save before closing?")
                .WithChoice("Yes", () => { _projectFileService.SaveProject(Project, false, newContinuation); })
                .WithChoice("No", () => { Project = null; newContinuation(); })
                .WithChoice("Cancel", () => { })
                .Build();
            _dialogHost.OpenDialog(saveBeforeCloseMessageBox);
        }

        public void CreateDiagram()
        {
            CreateDiagram(new DiagramModel());
        }

        public void CreateDiagram(DiagramModel diagram)
        {
            if (Project == null)
            {
                throw new NullReferenceException("ProjectModel does not exist");
            }

            if (diagram == null)
            {
                return;
            }

            var diagramName = string.IsNullOrEmpty(diagram.Name) ? "diagram" : diagram.Name;
            var diagramNumber = 1;
            while (Project.Diagrams.Any(x => x.Name.Equals(diagramName + diagramNumber)))
            {
                diagramNumber++;
            }

            diagram.Name = diagramName + diagramNumber;
            CreateDiagramViewModel(diagram);
            Project.AddDiagram(diagram);
        }

        public void CreateProject(Action continuation)
        {
            if (Project == null)
            {
                Project = new ProjectModel();
                CurrentProjectChanged?.Invoke();
                Project.IsDirty = false;
                continuation();
            }
            else
            {
                CloseProject(() =>
                {
                    Project = new ProjectModel();
                    CurrentProjectChanged?.Invoke();
                    Project.IsDirty = false;
                    continuation();
                });
            }
        }

        public void DeleteDiagram(DiagramModel diagram)
        {
            Project.RemoveDiagram(diagram);
            var diagramViewModel = Diagrams.FirstOrDefault(m => m.DiagramModel == diagram);
            if (diagramViewModel != null)
            {
                Diagrams.Remove(diagramViewModel);
            }
        }

        public void LoadProject(ProjectModel project, bool autoOpenDiagram = false)
        {
            CloseProject(() => LoadProjectInternal(project, autoOpenDiagram));
        }

        public void SaveAsProject()
        {
            if (Project != null)
            {
                _projectFileService.SaveProject(Project, true, () => { Project.IsDirty = false; });
            }
        }

        public void SaveProject()
        {
            if (Project != null)
            {
                _projectFileService.SaveProject(Project, false, () => { Project.IsDirty = false; });
            }
        }

        private void LoadProjectInternal(ProjectModel project, bool autoOpenDiagram)
        {
            Project = project;
            if (Project == null)
            {
                return;
            }

            try
            {
                CurrentProjectChanged?.Invoke();
                if (autoOpenDiagram)
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

                // TODO: Catch specific types of exceptions.
                throw;
            }
        }

        private void CreateDiagramViewModel(DiagramModel diagram)
        {
            var diagramViewModel = _diagramFactory.CreateDiagramViewModel(diagram);
            Diagrams.Add(diagramViewModel);
        }

        private async Task DownloadProjectDependencies()
        {
            foreach (var diagram in Project.Diagrams)
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
            Project.Diagrams?.ForEach(CreateDiagramViewModel);
        }
    }
}