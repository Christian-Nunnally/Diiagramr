using Diiagramr.Model;
using System;
using System.Collections.ObjectModel;

namespace Diiagramr.Service
{
    public interface IProjectManager
    {
        event Action CurrentProjectChanged;

        Project CurrentProject { get; set; }

        bool IsProjectDirty { get; set; }

        ObservableCollection<EDiagram> CurrentDiagrams { get; }

        void CreateProject();

        void SaveProject();

        void SaveAsProject();

        void LoadProject();

        bool CloseProject();

        void CreateDiagram();

        void DeleteDiagram(EDiagram diagram);
    }
}
