using DiiagramrAPI.Application;
using DiiagramrAPI.Application.Dialogs;
using DiiagramrAPI.Application.Tools;
using DiiagramrAPI.Editor.Diagrams;
using DiiagramrAPI.Service.Editor;
using DiiagramrAPI.Service.Plugins;
using DiiagramrCore;
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
        private ProjectModel _project;

        /// <summary>
        /// Creates a new instance of <see cref="ProjectManager"/>.
        /// </summary>
        /// <param name="dialogHostFactory">A factory that returns a <see cref="DialogHostBase"/>.</param>
        /// <param name="diagramFactoryFactory">A factory that returns an <see cref="DiagramFactory"/>.</param>
        /// <param name="libraryManagerFactory">A factory that returns an <see cref="ILibraryManager"/>.</param>
        /// <param name="projectFileServiceFactory">A factory that returns a <see cref="IProjectFileService"/>.</param>
        /// <param name="nodeServiceProviderFactory">A factory that returns a <see cref="NodeServiceProvider"/>.</param>
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

        /// <inheritdoc/>
        public IObservableCollection<Diagram> Diagrams { get; set; }

        /// <inheritdoc/>
        public ProjectModel Project
        {
            get => _project;
            set
            {
                _project = value;
                if (Project != null)
                {
                    Diagrams = new ViewModelCollection<Diagram, DiagramModel>(Project, () => Project?.Diagrams, _diagramFactory.CreateDiagramViewModel);
                }
            }
        }

        /// <inheritdoc/>
        public void CloseProject(Action continuation)
        {
            if (Project == null)
            {
                continuation();
                return;
            }

            void closeProjectAndContinue()
            {
                Diagrams.ForEach(d => d.Dispose());
                Diagrams = null;
                Project = null;
                continuation();
            }
            var saveBeforeCloseMessageBox = new MessageBox.Builder("Close Project", "Save before closing?")
                .WithChoice("Yes", () => _projectFileService.SaveProject(Project, false, closeProjectAndContinue))
                .WithChoice("No", () => closeProjectAndContinue())
                .WithChoice("Cancel", () => { })
                .Build();
            _dialogHost.OpenDialog(saveBeforeCloseMessageBox);
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public void CreateProject(Action continuation)
        {
            CloseProject(CreateProjectAndContinue(continuation));
        }

        /// <inheritdoc/>
        public void RemoveDiagram(DiagramModel diagram)
        {
            Project.RemoveDiagram(diagram);
        }

        /// <inheritdoc/>
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