using DiiagramrAPI.Application;
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
        private readonly SaveFileWindow _saveFileWindow;
        private readonly IProjectLoadSave _loadSave;
        private readonly IApplicationShell _applicationShell;

        public ProjectFileService(
            Func<IApplicationShell> applicationShellFactory,
            Func<IDirectoryService> directoryServiceFactory,
            Func<IProjectLoadSave> loadSaveFactory,
            Func<IDialogService> dialogServiceFactory,
            Func<SaveFileWindow> saveFileWindowFactory)
        {
            _applicationShell = applicationShellFactory.Invoke();
            _loadSave = loadSaveFactory.Invoke();
            _dialogService = dialogServiceFactory.Invoke();
            _saveFileWindow = saveFileWindowFactory.Invoke();

            var directoryService = directoryServiceFactory.Invoke();
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
            _applicationShell.OpenWindow(_saveFileWindow);

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
            }

            SerializeAndSave(project, ProjectDirectory + "\\" + project.Name + ProjectFileExtension);
        }

        private void SaveAsProject(ProjectModel project)
        {
            _saveFileWindow.InitialDirectory = ProjectDirectory;
            _saveFileWindow.FileName = project.Name;
            _saveFileWindow.SaveAction = fileName => SerializeAndSave(project, fileName);
            _applicationShell.OpenWindow(_saveFileWindow);
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
            var lastPeriod = path.LastIndexOf(".", StringComparison.Ordinal);
            project.Name = path.Substring(lastBackslashIndex + 1, lastPeriod - lastBackslashIndex - 1);
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