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
    public partial class LoadProjectDialog : Dialog
    {
        private string projectDirectory;
        private FileSystemWatcher _watcher;

        public LoadProjectDialog()
        {
            CommandBarCommands.Add(new DialogCommandBarCommand("Open Projects Directory", OpenProjectsDirectory));
        }

        public ObservableCollection<LoadProjectOption> LoadProjectOptions { get; set; } = new ObservableCollection<LoadProjectOption>();
        public override int MaxHeight => 220;

        public override int MaxWidth => 290;

        public override string Title { get; set; } = "Load Project";

        public string ProjectDirectory
        {
            get => projectDirectory;
            internal set
            {
                projectDirectory = value;

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

        public string FileName { get; internal set; }

        public Action<string> LoadAction { get; internal set; }

        public void OpenProjectsDirectory()
        {
            System.Diagnostics.Process.Start("explorer.exe", "Projects");
        }

        public void LoadProject()
        {
            LoadAction(FileName);
        }

        public void ProjectLoadOptionClicked(object sender, MouseButtonEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is LoadProjectOption dataContext)
            {
                CloseDialog();
                LoadAction(dataContext.Path);
            }
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