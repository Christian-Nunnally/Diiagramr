using DiiagramrAPI.Model;
using DiiagramrAPI.ViewModel.ProjectScreen.Diagram;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DiiagramrAPI.Service.Interfaces
{
    public interface IProjectManager : IDiiagramrService
    {
        event Action CurrentProjectChanged;

        ObservableCollection<DiagramModel> CurrentDiagrams { get; }
        ProjectModel CurrentProject { get; set; }

        IList<DiagramViewModel> DiagramViewModels { get; }
        bool IsProjectDirty { get; }

        bool CloseProject();

        void CreateDiagram();

        void CreateDiagram(DiagramModel diagram);

        void CreateProject();

        void DeleteDiagram(DiagramModel diagram);

        IEnumerable<Type> GetSerializeableTypes();

        void LoadProject(bool autoOpenDiagram = false);

        void SaveAsProject();

        void SaveProject();
    }
}
