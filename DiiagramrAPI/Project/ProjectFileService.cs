﻿using DiiagramrAPI.Application;
using DiiagramrAPI.Service.Dialog;
using DiiagramrAPI.Service.IO;
using DiiagramrAPI2.Application.Tools;
using DiiagramrModel;
using System;
using System.Linq;
using System.Windows;

namespace DiiagramrAPI.Project
{
    public class ProjectFileService : IProjectFileService
    {
        public const string ProjectFileExtension = ".xml";
        private readonly IDialogService _dialogService;
        private readonly SaveFileDialog _saveFileWindow;
        private readonly IProjectLoadSave _loadSave;
        private readonly DialogHost _dialogHost;

        public ProjectFileService(
            Func<IDirectoryService> directoryServiceFactory,
            Func<IProjectLoadSave> loadSaveFactory,
            Func<IDialogService> dialogServiceFactory,
            Func<SaveFileDialog> saveFileWindowFactory,
            Func<DialogHost> dialogHostFactory)
        {
            _loadSave = loadSaveFactory();
            _dialogService = dialogServiceFactory();
            _saveFileWindow = saveFileWindowFactory();
            _dialogHost = dialogHostFactory();

            var directoryService = directoryServiceFactory();
            ProjectDirectory = directoryService.GetCurrentDirectory() + "\\" + "Projects";

            if (!directoryService.Exists(ProjectDirectory))
            {
                directoryService.CreateDirectory(ProjectDirectory);
            }
        }

        public event Action<ProjectModel> ProjectSaved;

        public string ProjectDirectory { get; set; }

        public MessageBoxResult ConfirmProjectClose()
        {
            const string message = "Do you want to save before closing?";
            return _dialogService.Show(message, "Diiagramr", MessageBoxButton.YesNoCancel).Result;
        }

        public ProjectModel LoadProject()
        {
            _saveFileWindow.InitialDirectory = ProjectDirectory;
            _dialogHost.OpenDialog(_saveFileWindow);
            return LoadProject(_saveFileWindow.FileName);
        }

        public ProjectModel LoadProject(string path)
        {
            var project = _loadSave.Open(path);
            SetProjectNameFromPath(project, path);
            ThrowIfDuplicateAssemblies();
            return project;
        }

        public void SaveProject(ProjectModel project, bool saveAs)
        {
            if (saveAs || project.Name == "NewProject")
            {
                SaveAsProject(project);
                return;
            }

            SerializeAndSave(project, ProjectDirectory + "\\" + project.Name + ProjectFileExtension);
        }

        private void SaveAsProject(ProjectModel project)
        {
            _saveFileWindow.InitialDirectory = ProjectDirectory;
            _saveFileWindow.FileName = project.Name;
            _saveFileWindow.SaveAction = fileName => SerializeAndSave(project, fileName);
            _dialogHost.OpenDialog(_saveFileWindow);
        }

        private void SerializeAndSave(ProjectModel project, string fileName)
        {
            SetProjectNameFromPath(project, fileName);
            _loadSave.Save(project, fileName);
            ProjectSaved(project);
        }

        private void SetProjectNameFromPath(ProjectModel project, string path)
        {
            var lastBackslashIndex = path.LastIndexOf("\\", StringComparison.Ordinal);
            if (lastBackslashIndex == -1)
            {
                return;
            }

            ProjectDirectory = path.Substring(0, lastBackslashIndex);
            project.Name = path.Substring(lastBackslashIndex + 1, path.Length - lastBackslashIndex - 1);
        }

        private void ThrowIfDuplicateAssemblies()
        {
            var currentAssemblyNames = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.FullName);
            if (currentAssemblyNames.Distinct().Count() != currentAssemblyNames.Count())
            {
                throw new DuplicateAssemblyException();
            }
        }

        [Serializable]
        private class DuplicateAssemblyException : Exception
        {
        }
    }
}