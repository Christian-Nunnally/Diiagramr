using DiiagramrAPI.Model;
using DiiagramrAPI.Service;
using DiiagramrAPI.Service.Interfaces;
using Stylet;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace DiiagramrAPI.ViewModel
{
    public class ProjectExplorerViewModel : Screen
    {
        public ProjectExplorerViewModel(Func<IProjectManager> projectManagerFactory)
        {
            ProjectManager = projectManagerFactory.Invoke();
            ProjectManager.CurrentProjectChanged += ProjectManagerOnCurrentProjectChanged;
        }

        public IProjectManager ProjectManager { get; set; }

        public bool IsAddDiagramButtonVisible { get; set; }

        public ProjectModel Project { get; set; }

        public ObservableCollection<DiagramModel> Diagrams { get; set; } = new ObservableCollection<DiagramModel>();

        public DiagramModel SelectedDiagram { get; set; }

        public bool CanDeleteDiagram => SelectedDiagram != null;

        private void ProjectManagerOnCurrentProjectChanged()
        {
            IsAddDiagramButtonVisible = ProjectManager?.CurrentProject != null;
            Project = ProjectManager?.CurrentProject;
            Diagrams = ProjectManager?.CurrentDiagrams;
        }

        public void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed || SelectedDiagram == null)
            {
                return;
            }

            var dataObjectForDiagram = new DataObject(DataFormats.StringFormat, SelectedDiagram);
            DragDrop.DoDragDrop((UIElement)sender, dataObjectForDiagram, DragDropEffects.Copy);
        }

        public void DiagramProjectItemMouseUp()
        {
            foreach (var diagramModel in Diagrams)
            {
                diagramModel.IsOpen = false;
            }

            if (SelectedDiagram == null)
            {
                return;
            }

            SelectedDiagram.IsOpen = true;
        }

        public void CopyDiagram()
        {
            var copier = new DiagramCopier(ProjectManager);
            var diagramCopy = copier.Copy(SelectedDiagram);
            ProjectManager.CreateDiagram(diagramCopy);
        }

        public void CreateDiagram()
        {
            ProjectManager.CreateDiagram();
        }

        public void DeleteDiagram()
        {
            if (SelectedDiagram == null)
            {
                return;
            }

            var selectedDiagram = ProjectManager.CurrentDiagrams.First(x => x == SelectedDiagram);
            ProjectManager.DeleteDiagram(selectedDiagram);
        }
    }
}