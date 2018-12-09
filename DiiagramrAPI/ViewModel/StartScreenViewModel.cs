using DiiagramrAPI.Service.Interfaces;
using Stylet;
using System;
using System.Linq;

namespace DiiagramrAPI.ViewModel
{
    public class StartScreenViewModel : Screen
    {
        private readonly IProjectManager _projectManager;
        private bool _isMouseOverNewProjectButton;
        private bool _isMouseOverLoadProjectButton;

        public StartScreenViewModel(Func<IProjectManager> projectManagerFactory)
        {
            _projectManager = projectManagerFactory.Invoke();
        }

        public string NewButtonImageSource { get; set; } = "/Diiagramr;component/Resources/new.png";
        public string LoadButtonImageSource { get; set; } = "/Diiagramr;component/Resources/load.png";

        public void LoadProject()
        {
            if (Parent != null)
            {
                RequestClose();
            }

            _projectManager.LoadProject(autoOpenDiagram: true);
            if (_projectManager.CurrentProject == null)
            {
                LoadCanceled?.Invoke();
            }
        }

        public void NewProject()
        {
            if (Parent != null)
            {
                RequestClose();
            }

            _projectManager.CreateProject();
            _projectManager.CreateDiagram();
            _projectManager.CurrentDiagrams.First().IsOpen = true;
        }

        public void NewProjectButtonMouseEntered()
        {
            IsMouseOverNewProjectButton = true;
        }

        public bool IsMouseOverNewProjectButton
        {
            get => _isMouseOverNewProjectButton;
            set
            {
                _isMouseOverNewProjectButton = value;
                NewButtonImageSource = value ? "/Diiagramr;component/Resources/newHover.png" : "/Diiagramr;component/Resources/new.png";
            }
        }

        public void NewProjectButtonMouseLeft()
        {
            IsMouseOverNewProjectButton = false;
        }

        public void LoadProjectButtonMouseEntered()
        {
            IsMouseOverLoadProjectButton = true;
        }

        public bool IsMouseOverLoadProjectButton
        {
            get => _isMouseOverLoadProjectButton;
            set
            {
                _isMouseOverLoadProjectButton = value;
                LoadButtonImageSource = value ? "/Diiagramr;component/Resources/loadHover.png" : "/Diiagramr;component/Resources/load.png";
            }
        }

        public void LoadProjectButtonMouseLeft()
        {
            IsMouseOverLoadProjectButton = false;
        }

        public event Action LoadCanceled;
    }
}