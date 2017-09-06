using Diiagramr.Service;
using Diiagramr.Model;
using Stylet;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Diiagramr.ViewModel
{
    public class ProjectExplorerViewModel : Screen
    {
        private readonly IProjectManager _projectManager;

        public BindableCollection<Project> Projects { get; } = new BindableCollection<Project>();

        public ProjectExplorerViewModel(Func<IProjectManager> projectManagerFactory)
        {
            _projectManager = projectManagerFactory.Invoke();
            _projectManager.CurrentProjectChanged += ProjectManagerOnCurrentProjectChanged;
        }

        private void ProjectManagerOnCurrentProjectChanged()
        {
            Projects.Clear();
            if (_projectManager.CurrentProject != null)
            {
                Projects.Add(_projectManager.CurrentProject);
                _projectManager.CurrentProject.IsExpanded = true;
            }
        }

        public void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (SelectedDiagram != null)
                {
                    var dataObjectForDiagram = new DataObject(DataFormats.StringFormat, SelectedDiagram);
                    DragDrop.DoDragDrop((UIElement)sender, dataObjectForDiagram, DragDropEffects.Copy);
                }
            }
        }

        public EDiagram SelectedDiagram { get; set; }

        public Project SelectedProject { get; set; }

        public bool CanDeleteRequested => SelectedDiagram != null;

        public void CreateDiagram()
        {
            _projectManager.CreateDiagram();
        }

        private void SelectProject(Project selectedProject)
        {
            if (selectedProject == null) return;
            if (!Equals(selectedProject, SelectedProject))
            {
                SelectedProject = selectedProject;
                SelectedDiagram = null;
            }
        }

        private void SelectDiagram(EDiagram selectedDiagram)
        {
            if (selectedDiagram == null) return;
            if (Equals(selectedDiagram, SelectedDiagram)) return;
            SelectedDiagram = selectedDiagram;
            SelectedProject = null;
            SelectedDiagram.IsOpen = true;
        }

        public void SelectedItemChanged(object sender, EventArgs e)
        {
            if (!(e is RoutedPropertyChangedEventArgs<object>)) return;
            var er = (RoutedPropertyChangedEventArgs<object>)e;
            var selectedProject = er.NewValue as Project;
            var selectedDiagram = er.NewValue as EDiagram;
            SelectProject(selectedProject);
            SelectDiagram(selectedDiagram);
        }

        public void DeleteRequested()
        {
            if (SelectedDiagram == null) return;
            var selectedDiagram = _projectManager.CurrentDiagrams.First(x => x == SelectedDiagram);
            _projectManager.DeleteDiagram(selectedDiagram);
        }
    }
}