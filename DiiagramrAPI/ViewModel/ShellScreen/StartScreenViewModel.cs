using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiiagramrAPI.Service.Interfaces;
using Stylet;

namespace DiiagramrAPI.ViewModel.ShellScreen
{
    public class StartScreenViewModel : Screen
    {
        private IProjectManager _projectManager;

        public StartScreenViewModel(Func<IProjectManager> projectManagerFactory)
        {
            _projectManager = projectManagerFactory.Invoke();
        }

        public void LoadProject()
        {
            _projectManager.LoadProject();
            RequestClose();
        }

        public void NewProject()
        {
            _projectManager.CreateProject();
            RequestClose();
        }
    }
}
