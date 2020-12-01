using DiiagramrAPI.Application;
using DiiagramrCore;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace DiiagramrAPI2.Application.Dialogs
{
    /// <summary>
    /// A dialog that allows the user to load a project.
    /// </summary>
    public class LoadProjectDialog : Dialog
    {
        private string projectDirectory;
        private FileSystemWatcher _watcher;

        /// <summary>
        /// The list of recent project options present to the user in the dialog.
        /// </summary>
        public ObservableCollection<LoadProjectOption> LoadProjectOptions { get; set; } = new ObservableCollection<LoadProjectOption>();

        /// <inheritdoc/>
        public override int MaxHeight => 220;

        /// <inheritdoc/>
        public override int MaxWidth => 290;

        /// <inheritdoc/>
        public override string Title { get; set; }

        public string TitlePrefix { get; set; } = "Load";

        /// <summary>
        /// Gets or sets the directory to look for recent projects in.
        /// </summary>
        public string ProjectDirectory
        {
            get => projectDirectory;
            internal set
            {
                projectDirectory = value;
                var directoryName = ProjectDirectory.Split("\\").LastOrDefault();
                Title = $"{TitlePrefix} from {directoryName}";
                CommandBarCommands.Clear();
                CommandBarCommands.Add(new DialogCommandBarCommand($"Open {directoryName} Directory", OpenProjectsDirectory));
                LoadProjectOptions.Clear();
                UpdateProjectsList();

                _watcher?.Dispose();
                _watcher = new FileSystemWatcher
                {
                    Path = projectDirectory,
                    NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName,
                    Filter = "*.txt"
                };
                _watcher.Created += OnChanged;
                _watcher.Deleted += OnChanged;
                _watcher.Renamed += OnChanged;
                _watcher.EnableRaisingEvents = true;
            }
        }

        /// <summary>
        /// The name of the currently selected recent project.
        /// </summary>
        public string FileName { get; internal set; }

        /// <summary>
        /// An action that will load a project.
        /// </summary>
        public Action<string> LoadAction { get; internal set; }

        /// <summary>
        /// Occurs when the user clicks on the load project from disk button.
        /// </summary>
        public void LoadProject()
        {
            LoadAction(FileName);
        }

        /// <summary>
        /// Occurs when the user clicks on a specifc project button to load a recent project.
        /// </summary>
        /// <param name="sender">The recent project list item that was clicked on.</param>
        /// <param name="e">The event arguments.</param>
        public void ProjectLoadOptionClicked(object sender, MouseButtonEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is LoadProjectOption dataContext)
            {
                CloseDialog();
                LoadAction(dataContext.Path);
            }
        }

        private void OpenProjectsDirectory()
        {
            System.Diagnostics.Process.Start("explorer.exe", ProjectDirectory);
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            UpdateProjectsList();
        }

        private void UpdateProjectsList()
        {
            LoadProjectOptions.Clear();
            Directory
                .GetFiles(ProjectDirectory)
                .Select(LoadProjectOption.Create)
                .ForEach(LoadProjectOptions.Add);
        }
    }
}