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

        ProjectModel CurrentProject { get; set; }

        bool IsProjectDirty { get; }

        ObservableCollection<DiagramModel> CurrentDiagrams { get; }

        IList<DiagramViewModel> DiagramViewModels { get; }

        void CreateProject();

        void SaveProject();

        void SaveAsProject();

        void LoadProject(bool autoOpenDiagram = false);

        bool CloseProject();

        void CreateDiagram();

        void CreateDiagram(DiagramModel diagram);

        void DeleteDiagram(DiagramModel diagram);

        IEnumerable<Type> GetSerializeableTypes();
    }
}
