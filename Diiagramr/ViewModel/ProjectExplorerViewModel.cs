using Stylet;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Diiagramr.Model;
using Diiagramr.Service;
using Diiagramr.View.CustomControls;

namespace Diiagramr.ViewModel
{
    public class ProjectExplorerViewModel : Screen
    {
        private readonly DiagramWellViewModel _diagramWell;

        private readonly IProjectFileService _projectFileService;

        public ProjectExplorerViewModel(IProjectFileService projectFileService, DiagramWellViewModel diagramWell)
        {
            _diagramWell = diagramWell;
            Projects = new BindableCollection<Project>();
            _projectFileService = projectFileService;
            RenameRequestedHandler = RenameRequested;
        }

        public void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (SelectedDiagram != null)
                {
                    var dataObjectForDiagram = new DataObject(DataFormats.StringFormat, SelectedDiagram);
                    DragDrop.DoDragDrop((UIElement) sender, dataObjectForDiagram, DragDropEffects.Copy);
                }
            }
        }

        public BindableCollection<Project> Projects { get; set; }

        public Project CurrentProject { get; private set; }

        public Project SelectedProject { get; set; }

        public EDiagram SelectedDiagram { get; set; }

        public bool CanDeleteRequested => SelectedDiagram != null;

        public EditableTextBlock.RenameRequestedDelegate RenameRequestedHandler { get; set; }

        public bool IsExpanded { get; set; }

        public event Action ProjectChanged;

        public void SetProject(Project project)
        {
            CurrentProject = project;
            Projects.Add(CurrentProject);

            // Create a diagram on startup.
            if (CurrentProject != null && CurrentProject.Diagrams.Count == 0) CreateDiagram();
            IsExpanded = true;
        }

        public bool RenameRequested(string newName)
        {
            if (SelectedProject != null)
            {
                if (!_projectFileService.IsProjectNameValid(newName) || !RenameProject(newName)) return false;
                ProjectChanged?.Invoke();
                return true;
            }

            if (SelectedDiagram == null) return false;

            if (CurrentProject.Diagrams.Any(diagram => diagram.Name.Equals(newName)))
                return false;
            SelectedDiagram.Name = newName;
            ProjectChanged?.Invoke();
            return true;
        }

        private bool RenameProject(string name)
        {
            if (!_projectFileService.MoveProject(CurrentProject.Name, name)) return false;
            CurrentProject.Name = name;
            return true;
        }

        public void CreateDiagram()
        {
            if (CurrentProject == null) return;

            const string diagramName = "diagram";
            var diagramNumber = 1;
            var diagram = new EDiagram();
            while (CurrentProject.Diagrams.Any(x => x.Name.Equals(diagramName + diagramNumber)))
                diagramNumber++;
            diagram.Name = diagramName + diagramNumber;
            CurrentProject.Diagrams.Add(diagram);
            CurrentProject.IsExpanded = true;
            ProjectChanged?.Invoke();
        }

        public void SelectItem(Project selectedProject)
        {
            if (selectedProject == null) return;
            if (!Equals(selectedProject, SelectedProject))
            {
                SelectedProject = selectedProject;
                SelectedDiagram = null;
            }
        }

        public void SelectItem(EDiagram selectedDiagram)
        {
            if (selectedDiagram == null) return;
            if (Equals(selectedDiagram, SelectedDiagram)) return;
            SelectedDiagram = selectedDiagram;
            SelectedProject = null;
            _diagramWell.OpenDiagram(SelectedDiagram);
        }

        public void SelectedItemChanged(object sender, EventArgs e)
        {
            if (!(e is RoutedPropertyChangedEventArgs<object>)) return;
            var er = (RoutedPropertyChangedEventArgs<object>)e;
            var selectedProject = er.NewValue as Project;
            var selectedDiagram = er.NewValue as EDiagram;
            SelectItem(selectedProject);
            SelectItem(selectedDiagram);
        }


        public void CloseProject()
        {
            _diagramWell.CloseAllOpenDiagrams();
            Projects.Clear();
            CurrentProject = null;
        }

        public bool DeleteRequested()
        {
            if (SelectedDiagram == null) return false;
            var deletedDiagram = CurrentProject.Diagrams.First(x => x == SelectedDiagram);
            CurrentProject.Diagrams.Remove(deletedDiagram);
            deletedDiagram.Name = "";
            return true;
        }
    }
}