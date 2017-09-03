using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Diiagramr.Model;
using Diiagramr.Service;
using Stylet;

namespace Diiagramr.ViewModel
{
    public class ShellViewModel : Screen, IRequestClose, IGuardClose
    {
        private readonly IProjectFileService _projectFileService;

        public ShellViewModel(Func<IProjectFileService> projectFileServiceFactory, Func<DiagramWellViewModel> diagramWellViewModelFactory)
        {
            DiagramWellViewModel = diagramWellViewModelFactory.Invoke();
            _projectFileService = projectFileServiceFactory.Invoke();
            ProjectExplorerViewModel = new ProjectExplorerViewModel(_projectFileService, DiagramWellViewModel);
            _projectFileService.IsProjectNameValid("");
            SavePromptVisible = Visibility.Hidden;
            ProjectNamesInProjectDirectory = new ObservableCollection<string>();
            UpdateProjectNamesInProjectDirectory();

            // Create project on startup
            //CreateProject();
        }

        public bool ProjectSaved { get; set; }

        private bool SaveNotRequested { get; set; }

        public ProjectExplorerViewModel ProjectExplorerViewModel { get; set; }

        public DiagramWellViewModel DiagramWellViewModel { get; set; }

        public Visibility SavePromptVisible { get; set; }
        public bool IsClosed { get; private set; }

        public ObservableCollection<string> ProjectNamesInProjectDirectory { get; set; }

        public override Task<bool> CanCloseAsync()
        {
            var canClose = (ProjectExplorerViewModel?.CurrentProject == null) || ProjectSaved || SaveNotRequested;
            if (!canClose) SavePromptVisible = Visibility.Visible;
            return Task.FromResult(canClose);
        }

        public override void RequestClose(bool? dialogResult = null)
        {
            if (Parent != null) base.RequestClose(dialogResult);
            if ((ProjectExplorerViewModel?.CurrentProject == null) || ProjectSaved || SaveNotRequested) IsClosed = true;
            else IsClosed = false;

            if (IsClosed)
            {
                ProjectExplorerViewModel?.CloseProject();
                ProjectExplorerViewModel = null;
            }
            SavePromptVisible = Visibility.Visible;
        }

        public void CreateProject()
        {
            var count = 1;
            const string name = "project";
            var tempName = name + count;
            while (!_projectFileService.IsProjectNameValid(tempName))
            {
                count++;
                tempName = name + count;
            }

            var project = _projectFileService.CreateProject(tempName);
            SetProject(project);
            SaveProject();
        }

        public void LoadProject(string projectName)
        {
            var project = _projectFileService.LoadProject(projectName);
            SetProject(project);
        }

        private void SetProject(Project project)
        {
            if (project == null) throw new ProjectCreationException();
            ProjectExplorerViewModel.CloseProject();
            ProjectExplorerViewModel.SetProject(project);
            ProjectExplorerViewModel.ProjectChanged += ProjectChangedEventHandler;
        }

        public void SaveProject()
        {
            DiagramWellViewModel.SavingProject();

            ProjectSaved = true;
            _projectFileService.SaveProject(ProjectExplorerViewModel.CurrentProject);
        }

        public void Close()
        {
            SaveNotRequested = false;
            RequestClose();
        }

        public void DoNotSaveAndClose()
        {
            SaveNotRequested = true;
            RequestClose();
        }

        public void SaveAndClose()
        {
            SaveProject();
            RequestClose();
        }

        public void CancelClose()
        {
            SavePromptVisible = Visibility.Hidden;
        }

        public void ShellMouseDown()
        {
            SavePromptVisible = Visibility.Hidden;
        }

        public void UpdateProjectNamesInProjectDirectory()
        {
            var projectNames = _projectFileService.GetSavedProjectNames();
            ProjectNamesInProjectDirectory.Clear();
            foreach (var projectName in projectNames.Where(x => x.Contains("\\")))
                ProjectNamesInProjectDirectory.Add(projectName.Substring(projectName.LastIndexOf("\\", StringComparison.Ordinal) + 1));
        }

        public void ProjectChangedEventHandler()
        {
            ProjectSaved = false;
        }
    }

    public class ProjectCreationException : Exception
    {
    }
}