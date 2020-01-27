using DiiagramrAPI.Application;
using DiiagramrAPI.Application.Dialogs;
using DiiagramrAPI.Service.IO;
using DiiagramrAPI2.Application.Dialogs;
using DiiagramrModel;
using System;
using System.Linq;
using System.Threading;

namespace DiiagramrAPI.Project
{
    public class ProjectFileService : IProjectFileService
    {
        public const string ProjectFileExtension = ".xml";
        private readonly IProjectLoadSave _loadSave;
        private readonly SaveProjectDialog _saveProjectDialog;
        private readonly LoadProjectDialog _loadProjectDialog;
        private readonly DialogHost _dialogHost;

        public ProjectFileService(
            Func<IDirectoryService> directoryServiceFactory,
            Func<IProjectLoadSave> loadSaveFactory,
            Func<SaveProjectDialog> saveProjectDialogFactory,
            Func<LoadProjectDialog> loadProjectDialogFactory,
            Func<DialogHost> dialogHostFactory)
        {
            var directoryService = directoryServiceFactory();
            _loadSave = loadSaveFactory();
            _saveProjectDialog = saveProjectDialogFactory();
            _loadProjectDialog = loadProjectDialogFactory();
            _dialogHost = dialogHostFactory();

            ProjectDirectory = directoryService.GetCurrentDirectory() + "\\" + "Projects";

            if (!directoryService.Exists(ProjectDirectory))
            {
                directoryService.CreateDirectory(ProjectDirectory);
            }
        }

        public event Action<ProjectModel> ProjectSaved;

        public string ProjectDirectory { get; set; }

        public void LoadProject(Action<ProjectModel> continuation)
        {
            _loadProjectDialog.InitialDirectory = ProjectDirectory;
            _loadProjectDialog.LoadAction = s => { _dialogHost.CloseDialog(); continuation(LoadProject(s)); };
            _dialogHost.OpenDialog(_loadProjectDialog);
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

            var fileName = ProjectDirectory + "\\" + project.Name + ProjectFileExtension;
            SaveProjectWithNotificationDialog(project, fileName);
        }

        private void SaveAsProject(ProjectModel project)
        {
            _saveProjectDialog.InitialDirectory = ProjectDirectory;
            _saveProjectDialog.FileName = project.Name;
            _saveProjectDialog.SaveAction = fileName => SaveProjectWithNotificationDialog(project, fileName);
            _dialogHost.OpenDialog(_saveProjectDialog);
        }

        private void SaveProjectWithNotificationDialog(ProjectModel project, string fileName)
        {
            var notificationDialog = new NotificationDialog("Saving...");
            _dialogHost.OpenDialog(notificationDialog);
            SerializeAndSave(project, fileName);
            notificationDialog.Title = "Saved";
            new Thread(() =>
            {
                Thread.Sleep(600);
                _dialogHost.CloseDialog();
            }).Start();
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