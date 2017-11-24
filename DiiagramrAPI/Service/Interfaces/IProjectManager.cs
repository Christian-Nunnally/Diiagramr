using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DiiagramrAPI.Model;
using DiiagramrAPI.ViewModel.Diagram;
using DiiagramrAPI.ViewModel.ProjectScreen.Diagram;

namespace DiiagramrAPI.Service.Interfaces
{
    public interface IProjectManager
    {
        event Action CurrentProjectChanged;

        ProjectModel CurrentProject { get; set; }

        bool IsProjectDirty { get; }

        ObservableCollection<DiagramModel> CurrentDiagrams { get; }

        IList<DiagramViewModel> DiagramViewModels { get; }

        void CreateProject();

        void SaveProject();

        void SaveAsProject();

        void LoadProject();

        bool CloseProject();

        void CreateDiagram();

        void CreateDiagram(DiagramModel diagram);

        void DeleteDiagram(DiagramModel diagram);
    }
}
