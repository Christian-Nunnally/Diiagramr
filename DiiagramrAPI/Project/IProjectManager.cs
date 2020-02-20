using DiiagramrAPI.Editor.Diagrams;
using DiiagramrAPI.Service;
using DiiagramrModel;
using Stylet;
using System;

namespace DiiagramrAPI.Project
{
    public interface IProjectManager : ISingletonService
    {
        ProjectModel Project { get; set; }

        IObservableCollection<Diagram> Diagrams { get; }

        void CloseProject(Action contiuation);

        void InsertDiagram(DiagramModel diagram);

        void CreateProject(Action contiuation);

        void RemoveDiagram(DiagramModel diagram);

        void SetProject(ProjectModel project, bool autoOpenDiagram = false);
    }
}