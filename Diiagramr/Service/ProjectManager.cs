using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diiagramr.Model;

namespace DiagramEditor.Service
{
    public class ProjectManager : IProjectManager
    {
        public event Action CurrentProjectChanged;
        public Project CurrentProject { get; set; }
        public bool IsProjectDirty { get; set; }
        public ObservableCollection<EDiagram> CurrentDiagrams { get; }
        public void CreateProject()
        {
            //throw new NotImplementedException();
        }

        public void SaveProject()
        {
            //throw new NotImplementedException();
        }

        public void LoadProject(string path)
        {
            //throw new NotImplementedException();
        }

        public void CloseProject()
        {
            //throw new NotImplementedException();
        }

        public bool RenameProject(string newName)
        {
            return false;
            //throw new NotImplementedException();
        }

        public void CreateDiagram()
        {
            //throw new NotImplementedException();
        }

        public void DeleteDiagram(EDiagram diagram)
        {
            //throw new NotImplementedException();
        }
    }
}
