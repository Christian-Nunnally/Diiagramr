﻿using DiiagramrAPI.Project;
using DiiagramrAPI.Service.Interfaces;
using DiiagramrAPI.Shell;
using System;
using System.Linq;

namespace DiiagramrAPI.Service.Commands.FileCommands
{
    public class NewProjectCommand : ToolBarCommand
    {
        private readonly IProjectManager _projectManager;
        private readonly ProjectScreen _projectScreenViewModel;

        public override string Name => "New";
        public override string Parent => "Project";
        public override float Weight => 1.0f;

        public NewProjectCommand(Func<ProjectScreen> projectScreenViewModelFactory, Func<IProjectManager> projectManagerFactory)
        {
            _projectManager = projectManagerFactory.Invoke();
            _projectScreenViewModel = projectScreenViewModelFactory.Invoke();
        }

        internal override void ExecuteInternal(IShell shell, object parameter)
        {
            _projectManager.CreateProject();
            _projectManager.CreateDiagram();
            _projectManager.CurrentDiagrams.First().IsOpen = true;
            shell.ShowScreen(_projectScreenViewModel);
        }
    }
}
