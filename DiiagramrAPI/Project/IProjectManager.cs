using DiiagramrAPI.Editor.Diagrams;
using DiiagramrAPI.Service;
using DiiagramrModel;
using System;
using System.Collections.ObjectModel;

namespace DiiagramrAPI.Project
{
    public interface IProjectManager : ISingletonService
    {
        ProjectModel Project { get; set; }

        ObservableCollection<Diagram> Diagrams { get; }

        void CloseProject(Action contiuation);

        void CreateDiagram();

        void CreateDiagram(DiagramModel diagram);

        void CreateProject(Action contiuation);

        void DeleteDiagram(DiagramModel diagram);

        void SetProject(ProjectModel project, bool autoOpenDiagram = false);
    }
}