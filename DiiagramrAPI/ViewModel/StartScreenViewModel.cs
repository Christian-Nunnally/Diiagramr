using System;
using System.Linq;
using DiiagramrAPI.Service.Interfaces;
using Stylet;

namespace DiiagramrAPI.ViewModel.ShellScreen
{
    public class StartScreenViewModel : Screen
    {
        private readonly IProjectManager _projectManager;

        public StartScreenViewModel(Func<IProjectManager> projectManagerFactory)
        {
            _projectManager = projectManagerFactory.Invoke();
        }

        public void LoadProject()
        {
            if (Parent == null) return;
            RequestClose();
            _projectManager.LoadProject();
        }

        public void NewProject()
        {
            if (Parent == null) return;
            RequestClose();
            _projectManager.CreateProject();
            _projectManager.CreateDiagram();
            _projectManager.CurrentDiagrams.First().IsOpen = true;
        }
    }
}