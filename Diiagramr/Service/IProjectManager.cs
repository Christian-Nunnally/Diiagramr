using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Diiagramr.Model;

namespace DiagramEditor.Service
{
    public interface IProjectManager
    {
        event Action CurrentProjectChanged;

        Project CurrentProject { get; set; }

        bool IsProjectDirty { get; set; }

        ObservableCollection<EDiagram> CurrentDiagrams { get; }

        void CreateNewProject();

        void SaveProject();

        void LoadProject(string path);

        void CloseProject();

        bool RenameProject(string newName);
    }
}
