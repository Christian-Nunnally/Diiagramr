using DiiagramrAPI.Application;
using System;

namespace DiiagramrAPI.Project
{
    public class ProjectScreen : ViewModel
    {
        public ProjectScreen(Func<DiagramWell> diagramWellFactory)
        {
            DiagramWell = diagramWellFactory();
        }

        public DiagramWell DiagramWell { get; set; }
    }
}