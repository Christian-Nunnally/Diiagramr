using DiiagramrAPI.Editor.Diagrams;
using DiiagramrAPI.Service;
using DiiagramrModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DiiagramrAPI.Project
{
    public interface IProjectManager : IService
    {
        event Action CurrentProjectChanged;

        ObservableCollection<DiagramModel> CurrentDiagrams { get; }

        ProjectModel CurrentProject { get; set; }

        IList<Diagram> Diagrams { get; }

        bool IsProjectDirty { get; }

        bool CloseProject();

        void CreateDiagram();

        void CreateDiagram(DiagramModel diagram);

        void CreateProject();

        void DeleteDiagram(DiagramModel diagram);

        IEnumerable<Type> GetSerializeableTypes();

        void LoadProject(ProjectModel project, bool autoOpenDiagram = false);

        void SaveAsProject();

        void SaveProject();
    }
}