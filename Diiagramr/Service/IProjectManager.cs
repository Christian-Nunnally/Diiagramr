using Diiagramr.Model;
using System;
using System.Collections.ObjectModel;

namespace DiagramEditor.Service
{
    public interface IProjectManager
    {
        event Action CurrentProjectChanged;

        Project CurrentProject { get; set; }

        bool IsProjectDirty { get; set; }

        ObservableCollection<EDiagram> CurrentDiagrams { get; }

        void CreateProject();

        void SaveProject();

        void LoadProject(string path);

        void CloseProject();

        bool RenameProject(string newName);

        void CreateDiagram();

        void DeleteDiagram(EDiagram diagram);
    }
}
