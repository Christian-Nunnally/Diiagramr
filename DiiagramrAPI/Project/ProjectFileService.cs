using DiiagramrAPI.Application;
using DiiagramrAPI.Application.Dialogs;
using DiiagramrAPI.Service.IO;
using DiiagramrAPI2.Application.Dialogs;
using DiiagramrModel;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;

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
            _loadProjectDialog.ProjectDirectory = ProjectDirectory;
            _loadProjectDialog.LoadAction = s => { _dialogHost.CloseDialog(); continuation(LoadProject(s)); };
            _dialogHost.OpenDialog(_loadProjectDialog);
        }

        public ProjectModel LoadProject(string path)
        {
            try
            {
                var project = _loadSave.Load(path);
                SetProjectNameFromPath(project, path);
                ThrowIfDuplicateAssemblies();
                return project;
            }
            catch (DuplicateAssemblyException e)
            {
                DisplayErrorMessageBox("Error Loading Project", "Duplicate assemblies loaded. Make sure the plugins directly only has one copy of each DLL.");
            }
            catch (XmlException e)
            {
                DisplayErrorMessageBox("Error Loading Project", "Error Parsing XML: " + e.Message);
            }
            return null;
        }

        public void SaveProject(ProjectModel project, bool saveAs, Action continuation)
        {
            var fileName = ProjectDirectory + "\\" + project.Name + ProjectFileExtension;
            if (saveAs)
            {
                SaveAsProject(project, continuation);
                return;
            }
            SaveProjectWithNotificationDialog(project, fileName, continuation);
        }

        private void DisplayErrorMessageBox(string title, string message)
        {
            var errorMessage = new MessageBox.Builder(title, message).WithChoice("Ok", () => { }).Build();
            _dialogHost.OpenDialog(errorMessage);
        }

        private void SaveAsProject(ProjectModel project, Action continuation)
        {
            _saveProjectDialog.InitialDirectory = ProjectDirectory;
            _saveProjectDialog.ProjectName = project.Name;
            _saveProjectDialog.SaveAction = fileName => SaveProjectAndPromptIfOverwriting(project, fileName, saveAs: true, continuation);
            _dialogHost.OpenDialog(_saveProjectDialog);
        }

        private void SaveProjectAndPromptIfOverwriting(ProjectModel project, string fileName, bool saveAs, Action continuation)
        {
            if (saveAs && File.Exists(fileName))
            {
                var overwriteConfirmationPrompt = new MessageBox.Builder("Overwrite Project?", $"A project already exists at {fileName}. Do you want to overwrite it?")
                        .WithChoice("Yes", () => SaveProjectWithNotificationDialog(project, fileName, continuation))
                        .WithChoice("No", () => SaveAsProject(project, continuation)).Build();
                _dialogHost.OpenDialog(overwriteConfirmationPrompt);
                return;
            }
            SaveProjectWithNotificationDialog(project, fileName, continuation);
        }

        private void SaveProjectWithNotificationDialog(ProjectModel project, string fileName, Action continuation)
        {
            var notificationDialog = new NotificationDialog("Saving...");
            _dialogHost.OpenDialog(notificationDialog);
            SerializeAndSave(project, fileName);
            notificationDialog.Title = "Saved";
            new Thread(() =>
            {
                Thread.Sleep(500);
                _dialogHost.CloseDialog();
                continuation();
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