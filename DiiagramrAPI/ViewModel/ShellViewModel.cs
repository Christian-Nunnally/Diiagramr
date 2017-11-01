using System;
using DiiagramrAPI.Service.Interfaces;
using DiiagramrAPI.ViewModel.ShellScreen;
using DiiagramrAPI.ViewModel.ShellScreen.ProjectScreen;
using Stylet;

namespace DiiagramrAPI.ViewModel
{
    public class ShellViewModel : Conductor<IScreen>.StackNavigation
    {
        private readonly IProjectManager _projectManager;

        public LibraryManagerScreenViewModel LibraryManagerScreenViewModel { get; set; }
        public ProjectScreenViewModel ProjectScreenViewModel { get; set; }

        public bool CanSaveProject { get; set; }
        public bool CanSaveAsProject { get; set; }

        public string WindowTitle { get; set; } = "Diiagramr";

        public ShellViewModel(
            Func<ProjectScreenViewModel> projectScreenViewModelFactory,
            Func<LibraryManagerScreenViewModel> libraryScreenScreenViewModelFactory,
            Func<IProjectManager> projectManagerFactory)
        {
            ProjectScreenViewModel = projectScreenViewModelFactory.Invoke();
            LibraryManagerScreenViewModel = libraryScreenScreenViewModelFactory.Invoke();

            _projectManager = projectManagerFactory.Invoke();
            _projectManager.CurrentProjectChanged += ProjectManagerOnCurrentProjectChanged;

            ShowScreen(ProjectScreenViewModel);
        }

        public void ShowScreen(IScreen screen)
        {
            ActiveItem = screen;
        }

        private void ProjectManagerOnCurrentProjectChanged()
        {
            CanSaveProject = _projectManager.CurrentProject != null;
            CanSaveAsProject = _projectManager.CurrentProject != null;
            WindowTitle = "Diiagramr" + (_projectManager.CurrentProject != null ? " - " + _projectManager.CurrentProject.Name : "");
        }

        public override void RequestClose(bool? dialogResult = null)
        {
            if (_projectManager.CloseProject())
            {
                if (Parent != null) base.RequestClose(dialogResult);
            }
        }

        public void CreateProject()
        {
            _projectManager.CreateProject();
        }

        public void LoadProject()
        {
            _projectManager.LoadProject();
        }

        public void SaveProject()
        {
            _projectManager.SaveProject();
        }

        public void SaveAsProject()
        {
            _projectManager.SaveAsProject();
        }

        // TODO: Unit test
        public void ManageLibraries()
        {
            if (ActiveItem == LibraryManagerScreenViewModel)
                ActiveItem = ProjectScreenViewModel;
            else
                ActiveItem = LibraryManagerScreenViewModel;
        }

        public void Close()
        {
            RequestClose();
        }

        public void SaveAndClose()
        {
            _projectManager.SaveProject();
            RequestClose();
        }
    }
}