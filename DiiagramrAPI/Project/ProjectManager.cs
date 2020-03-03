using DiiagramrAPI.Application;
using DiiagramrAPI.Application.Tools;
using DiiagramrAPI.Editor.Diagrams;
using DiiagramrAPI.Service.Editor;
using DiiagramrAPI.Service.Plugins;
using DiiagramrModel;
using Stylet;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DiiagramrAPI.Project
{
    /// <summary>
    /// Manages the lifecycle of an open project.
    /// </summary>
    public class ProjectManager : IProjectManager
    {
        private readonly DiagramFactory _diagramFactory;
        private readonly DialogHostBase _dialogHost;
        private readonly ILibraryManager _libraryManager;
        private readonly IProjectFileService _projectFileService;
        private readonly NodeServiceProvider _nodeServiceProvider;
        private ProjectModel project;

        public ProjectManager(
            Func<DialogHostBase> dialogHostFactory,
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
            _nodeServiceProvider.RegisterService<IProjectManager>(this);
        }

        public IObservableCollection<Diagram> Diagrams { get; set; }

        public ProjectModel Project
        {
            get => project;
            set
            {
                project = value;
                Diagrams = new ViewModelCollection<Diagram, DiagramModel>(Project, () => Project?.Diagrams, _diagramFactory.CreateDiagramViewModel);
            }
        }

        public void CloseProject(Action continuation)
        {
            if (Project == null)
            {
                continuation();
                return;
            }

            void newContinuation() { Project = null; continuation(); }
            var saveBeforeCloseMessageBox = new Application.Dialogs.MessageBox.Builder("Close Project", "Save before closing?")
                .WithChoice("Yes", () => _projectFileService.SaveProject(Project, false, newContinuation))
                .WithChoice("No", () => newContinuation())
                .WithChoice("Cancel", () => { })
                .Build();
            _dialogHost.OpenDialog(saveBeforeCloseMessageBox);
        }

        public void InsertDiagram(DiagramModel diagram)
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
            Project.AddDiagram(diagram);
        }

        public void CreateProject(Action continuation)
        {
            CloseProject(CreateProjectAndContinue(continuation));
        }

        public void RemoveDiagram(DiagramModel diagram)
        {
            Project.RemoveDiagram(diagram);
        }

        public void SetProject(ProjectModel project, bool autoOpenDiagram = false)
        {
            CloseProject(() => SetProjectInternal(project, autoOpenDiagram));
        }

        private Action CreateProjectAndContinue(Action continuation) => () =>
        {
            Project = new ProjectModel();
            continuation();
        };

        private void SetProjectInternal(ProjectModel project, bool autoOpenDiagram)
        {
            if (project == null)
            {
                return;
            }

            try
            {
                Project = project;
                if (autoOpenDiagram && !project.Diagrams.Any())
                {
                    InsertDiagram(new DiagramModel());
                }
            }
            catch (Exception)
            {
                // This code is so that if a project fails to load because it contains a node from an uninstalled library,
                // it will attempt to download and install the missing library. I'm pretty sure this code work as intended
                // because nodes that fail to load are handled by printing a message to the output and just ignoring that node.
                // TODO: Make async
                DownloadProjectDependencies().Wait();
                Project = project;
                throw;
            }
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
    }
}