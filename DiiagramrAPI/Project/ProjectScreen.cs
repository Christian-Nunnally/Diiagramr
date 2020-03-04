using DiiagramrAPI.Application;
using DiiagramrAPI.Editor.Diagrams;
using DiiagramrAPI.Service.Editor;
using System;

namespace DiiagramrAPI.Project
{
    public class ProjectScreen : ViewModel, IUserInputBeforeClosedRequest
    {
        private readonly IProjectManager _projectManager;

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