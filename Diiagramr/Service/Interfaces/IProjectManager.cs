using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Diiagramr.Model;
using Diiagramr.ViewModel.Diagram;

namespace Diiagramr.Service.Interfaces
{
    public interface IProjectManager
    {
        event Action CurrentProjectChanged;

        ProjectModel CurrentProject { get; set; }

        bool IsProjectDirty { get; set; }

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
