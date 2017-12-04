using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using DiiagramrAPI.Service.Interfaces;
using DiiagramrAPI.ViewModel.ProjectScreen;
using DiiagramrAPI.ViewModel.ShellScreen;
using Stylet;

namespace DiiagramrAPI.ViewModel
{
    public class ShellViewModel : Conductor<IScreen>.StackNavigation
    {
        private readonly IProjectManager _projectManager;

        public LibraryManagerWindowViewModel LibraryManagerWindowViewModel { get; set; }
        public ProjectScreenViewModel ProjectScreenViewModel { get; set; }
        public StartScreenViewModel StartScreenViewModel { get; set; }

        public bool CanSaveProject { get; set; }
        public bool CanSaveAsProject { get; set; }

        public string WindowTitle { get; set; } = "Diiagramr";

        public Stack<AbstractShellWindow> WindowStack = new Stack<AbstractShellWindow>();
        public AbstractShellWindow ActiveWindow { get; set; }

        public bool IsWindowOpen => ActiveWindow != null;

        public ShellViewModel(
            Func<ProjectScreenViewModel> projectScreenViewModelFactory,
            Func<LibraryManagerWindowViewModel> libraryManagerWindowViewModelFactory,
            Func<StartScreenViewModel> startScreenScreenViewModelFactory,
            Func<IProjectManager> projectManagerFactory)
        {
            ProjectScreenViewModel = projectScreenViewModelFactory.Invoke();
            LibraryManagerWindowViewModel = libraryManagerWindowViewModelFactory.Invoke();
            StartScreenViewModel = startScreenScreenViewModelFactory.Invoke();

            _projectManager = projectManagerFactory.Invoke();
            _projectManager.CurrentProjectChanged += ProjectManagerOnCurrentProjectChanged;

            ShowScreen(ProjectScreenViewModel);
            ShowScreen(StartScreenViewModel);
        }

        public void ShowScreen(IScreen screen)
        {
            ActiveItem = screen;
        }

        public void OpenWindow(AbstractShellWindow window)
        {
            window.OpenWindow += OpenWindow;
            if (ActiveWindow != null) WindowStack.Push(ActiveWindow);
            ActiveWindow = window;
        }

        public void CloseWindow()
        {
            ActiveWindow.OpenWindow -= OpenWindow;
            ActiveWindow = WindowStack.Count > 0 ? WindowStack.Pop() : null;
        }

        private void ProjectManagerOnCurrentProjectChanged()
        {
            if (_projectManager.CurrentProject == null) ShowScreen(StartScreenViewModel);
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
        
        // TODO: Move to some other class.  The other other class should basically be abl to add toolbar buttons.
        public void ManageLibraries()
        {
            OpenWindow(LibraryManagerWindowViewModel);
        }

        public void Close()
        {
            if (_projectManager.CloseProject())
            {
                ShowScreen(StartScreenViewModel);
            }
        }

        public void WindowClosing(object sender, CancelEventArgs e)
        {
            if (!_projectManager.CloseProject()) e.Cancel = true;
        }
    }
}