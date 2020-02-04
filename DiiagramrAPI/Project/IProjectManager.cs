using DiiagramrAPI.Editor.Diagrams;
using DiiagramrAPI.Service;
using DiiagramrModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DiiagramrAPI.Project
{
    public interface IProjectManager : ISingletonService
    {
        event Action CurrentProjectChanged;

        ObservableCollection<DiagramModel> CurrentDiagrams { get; }

        ProjectModel Project { get; set; }

        ObservableCollection<Diagram> Diagrams { get; }

        bool IsProjectDirty { get; }

        void CloseProject(Action contiuation);

        void CreateDiagram();

        void CreateDiagram(DiagramModel diagram);

        void CreateProject(Action contiuation);

        void DeleteDiagram(DiagramModel diagram);

        IEnumerable<Type> GetSerializeableTypes();

        void LoadProject(ProjectModel project, bool autoOpenDiagram = false);

        void SaveAsProject();

        void SaveProject();
    }
}