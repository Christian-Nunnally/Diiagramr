using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diiagramr.Model;
using System.IO;
using Diiagramr.Service;

namespace Diiagramr.Service
{
    public class ProjectManager : IProjectManager
    {
        public event Action CurrentProjectChanged;
        public Project CurrentProject { get; set; }
        public bool IsProjectDirty { get; set; }
        public ObservableCollection<EDiagram> CurrentDiagrams => CurrentProject?.Diagrams;
        public IProjectFileService _projectFileService;

        public ProjectManager(IProjectFileService projectFileService)
        {
            _projectFileService = projectFileService;
        }

        public void CreateProject()
        {
            CurrentProject = new Project("Project");
            IsProjectDirty = true;
            CurrentProjectChanged.Invoke();
            _projectFileService.ProjectName = null;
        }

        public void SaveProject()
        {
            if (_projectFileService.SaveProject(CurrentProject, false))
            {
                IsProjectDirty = false;
            }
        }

        public void SaveAsProject()
        {
            if (_projectFileService.SaveProject(CurrentProject, true))
            {
                IsProjectDirty = false;
            }
        }

        public void LoadProject()
        {
            CurrentProject = _projectFileService.LoadProject();
            CurrentProjectChanged.Invoke();
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
