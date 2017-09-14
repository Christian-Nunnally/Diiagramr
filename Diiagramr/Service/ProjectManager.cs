using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diiagramr.Model;
using System.IO;
using System.Windows.Forms;
using Diiagramr.Service;

namespace Diiagramr.Service
{
    public class ProjectManager : IProjectManager
    {
        public event Action CurrentProjectChanged;
        public Project CurrentProject { get; set; }
        public bool IsProjectDirty { get; set; }
        public ObservableCollection<DiagramModel> CurrentDiagrams => CurrentProject?.Diagrams;
        public IProjectFileService _projectFileService;

        public ProjectManager(IProjectFileService projectFileService)
        {
            _projectFileService = projectFileService;
        }

        public void CreateProject()
        {
            if (CloseProject())
            {
                CurrentProject = new Project();
                IsProjectDirty = true;
                CurrentProjectChanged.Invoke();
            }
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
            if (CloseProject())
            {
                CurrentProject = _projectFileService.LoadProject();
                IsProjectDirty = false;
                CurrentProjectChanged.Invoke();
            }
        }

        public bool CloseProject()
        {
            if (IsProjectDirty)
            {
                var result = _projectFileService.ConfirmProjectClose();
                if (result == DialogResult.Cancel)
                {
                    return false;
                }
                else if (result == DialogResult.Yes)
                {
                    _projectFileService.SaveProject(CurrentProject, false);
                }
            }
            return true;
        }

        public void CreateDiagram()
        {
            if (CurrentProject == null)
            {
                throw new NullReferenceException("Project does not exist");
            }
            const string dName = "diagram";
            var dNum = 1;
            var diagram = new DiagramModel();
            while (CurrentProject.Diagrams.Any(x => x.Name.Equals(dName + dNum)))
                dNum++;
            diagram.Name = dName + dNum;
            IsProjectDirty = true;
            CurrentProject.Diagrams.Add(diagram);
            CurrentProjectChanged.Invoke();
        }

        public void DeleteDiagram(DiagramModel diagram)
        {
            var dToDelete = CurrentDiagrams.First(x => x == diagram);
            CurrentProject.Diagrams.Remove(dToDelete);

            IsProjectDirty = true;
            CurrentProjectChanged.Invoke();
        }
    }
}
