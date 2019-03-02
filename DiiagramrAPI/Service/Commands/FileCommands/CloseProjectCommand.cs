﻿using DiiagramrAPI.Shell;
using DiiagramrAPI.ViewModel;
using System;

namespace DiiagramrAPI.Service.Commands.FileCommands
{
    public class CloseProjectCommand : DiiagramrCommand
    {
        private readonly StartScreenViewModel _startScreenViewModel;
        private readonly ProjectManager _projectManager;

        public CloseProjectCommand(Func<StartScreenViewModel> startScreenViewModelFactory, Func<ProjectManager> projectManagerFactory)
        {
            _startScreenViewModel = startScreenViewModelFactory.Invoke();
            _projectManager = projectManagerFactory.Invoke();
        }

        public override string Name => "Close";
        public override string Parent => "Project";

        internal override void ExecuteInternal(IShell shell, object parameter)
        {
            if (_projectManager.CloseProject())
            {
                shell.ShowScreen(_startScreenViewModel);
            }
        }
    }
}
