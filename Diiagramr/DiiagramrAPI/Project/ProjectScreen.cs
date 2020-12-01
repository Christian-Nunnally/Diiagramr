using DiiagramrAPI.Application;
using DiiagramrAPI.Editor.Diagrams;
using DiiagramrAPI.Service.Editor;
using Stylet;
using System;

namespace DiiagramrAPI.Project
{
    /// <summary>
    /// The screen that contains everything for an open project.
    /// </summary>
    public class ProjectScreen : Screen, IUserInputBeforeClosedRequest
    {
        private readonly IProjectManager _projectManager;

        /// <summary>
        /// Creates a new instance of <see cref="ProjectScreen"/>.
        /// </summary>
        /// <param name="diagramWellFactory">A factory that returns a <see cref="DiagramWell"/>.</param>
        /// <param name="nodeServideProviderFactory">A factory that returns an <see cref="NodeServiceProvider"/>.</param>
        /// <param name="projectManagerFactory">A factory that returns an <see cref="IProjectManager"/>.</param>
        public ProjectScreen(
            Func<DiagramWell> diagramWellFactory,
            Func<NodeServiceProvider> nodeServideProviderFactory,
            Func<IProjectManager> projectManagerFactory)
        {
            _projectManager = projectManagerFactory();
            DiagramWell = diagramWellFactory();
            nodeServideProviderFactory().RegisterService(this);
        }

        public DiagramWell DiagramWell { get; set; }

        public void ContinueIfCanClose(Action continuation)
        {
            _projectManager.CloseProject(closeDiagramsAndContinue);

            void closeDiagramsAndContinue()
            {
                DiagramWell.CloseAllDiagrams();
                continuation();
            }
        }

        public void OpenDiagram(Diagram diagram)
        {
            DiagramWell.OpenDiagram(diagram);
        }
    }
}