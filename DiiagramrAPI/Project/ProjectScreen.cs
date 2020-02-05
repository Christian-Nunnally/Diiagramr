using DiiagramrAPI.Application;
using DiiagramrAPI.Editor.Diagrams;
using DiiagramrAPI.Service.Editor;
using System;

namespace DiiagramrAPI.Project
{
    public class ProjectScreen : ViewModel
    {
        public ProjectScreen(
            Func<DiagramWell> diagramWellFactory,
            Func<NodeServiceProvider> nodeServideProviderFactory)
        {
            DiagramWell = diagramWellFactory();
            nodeServideProviderFactory().RegisterService(this);
        }

        public DiagramWell DiagramWell { get; set; }

        public void OpenDiagram(Diagram diagram)
        {
            DiagramWell.OpenDiagram(diagram);
        }
    }
}