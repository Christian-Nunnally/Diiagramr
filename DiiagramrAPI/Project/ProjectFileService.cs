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
    /// <summary>
    /// Saves and loads projects from the file system.
    /// </summary>
    public class ProjectFileService : IProjectFileService
    {
        /// <summary>
        /// The extension to save project files with.
        /// </summary>
        public const string ProjectFileExtension = ".xml";

        public static string ProjectsSaveDirectoryPath;
        public static string TemplatesSaveDirectoryPath;
        private readonly IDirectoryService _directoryService;
        private readonly IProjectLoadSave _loadSave;
        private readonly SaveProjectDialog _saveProjectDialog;
        private readonly LoadProjectDialog _loadProjectDialog;
        private readonly DialogHostBase _dialogHost;
        private string directoryToService;

        /// <summary>
        /// Creates a new instance of <see cref="ProjectFileService"/>.
        /// </summary>
        /// <param name="directoryServiceFactory">A factory that returns an <see cref="IDirectoryService"/>.</param>
        /// <param name="loadSaveFactory">A factory that returns an <see cref="IProjectLoadSave"/>.</param>
        /// <param name="saveProjectDialogFactory">A factory that returns a <see cref="SaveProjectDialog"/>.</param>
        /// <param name="loadProjectDialogFactory">A factory that returns a <see cref="LoadProjectDialog"/>.</param>
        /// <param name="dialogHostFactory">A factory that returns a <see cref="DialogHostBase"/>.</param>
        public ProjectFileService(
            Func<IDirectoryService> directoryServiceFactory,
            Func<IProjectLoadSave> loadSaveFactory,
            Func<SaveProjectDialog> saveProjectDialogFactory,
            Func<LoadProjectDialog> loadProjectDialogFactory,
            Func<DialogHostBase> dialogHostFactory)
        {
            _directoryService = directoryServiceFactory();
            _loadSave = loadSaveFactory();
            _saveProjectDialog = saveProjectDialogFactory();
            _loadProjectDialog = loadProjectDialogFactory();
            _dialogHost = dialogHostFactory();

            // TODO: Get rid of all three of these lines.
            ProjectsSaveDirectoryPath = _directoryService.GetCurrentDirectory() + "\\" + "Projects";
            TemplatesSaveDirectoryPath = _directoryService.GetCurrentDirectory() + "\\" + "Templates";
            DirectoryToService = ProjectsSaveDirectoryPath; // Default to the project directory. Probably want to remove this.
        }

        /// <inheritdoc/>
        public string DirectoryToService
        {
            get => directoryToService;
            set
            {
                directoryToService = value;
                if (!_directoryService.Exists(directoryToService))
                {
                    _directoryService.CreateDirectory(directoryToService);
                }
            }
        }

        /// <inheritdoc/>
        public void LoadProject(Action<ProjectModel> continuation)
        {
            _loadProjectDialog.TitlePrefix = DirectoryToService.EndsWith("Projects") ? "Load" : "Create";
            _loadProjectDialog.ProjectDirectory = DirectoryToService;
            _loadProjectDialog.LoadAction = s => { _dialogHost.CloseDialog(); continuation(LoadProject(s)); };
            _dialogHost.OpenDialog(_loadProjectDialog);
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public void SaveProject(ProjectModel project, bool saveAs, Action continuation)
        {
            var projectName = project.Name;
            projectName += project.Name.EndsWith(ProjectFileExtension) ? "" : ProjectFileExtension;
            var fileName = DirectoryToService + "\\" + projectName;
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
            _saveProjectDialog.InitialDirectory = DirectoryToService;
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
        }

        private void SetProjectNameFromPath(ProjectModel project, string path)
        {
            var lastBackslashIndex = path.LastIndexOf("\\", StringComparison.Ordinal);
            if (lastBackslashIndex == -1)
            {
                return;
            }

            ProjectsSaveDirectoryPath = path.Substring(0, lastBackslashIndex);
            var name = path.Substring(lastBackslashIndex + 1, path.Length - lastBackslashIndex - 1);
            while (name.EndsWith($".{ProjectFileExtension}.{ProjectFileExtension}")) name = name.Remove(name.Length - 5);
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