﻿using DiiagramrAPI.Application;
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

        public LoadProjectDialog()
        {
            CommandBarCommands.Add(new DialogCommandBarCommand("Open Projects Directory", OpenProjectsDirectory));
        }

        public ObservableCollection<LoadProjectOption> LoadProjectOptions { get; set; } = new ObservableCollection<LoadProjectOption>();
        public override int MaxHeight => 300;

        public override int MaxWidth => 300;

        public override string Title { get; set; } = "Load Project";

        public string ProjectDirectory
        {
            get => projectDirectory;
            internal set
            {
                Directory
                    .GetFiles(value)
                    .Select(LoadProjectOption.Create)
                    .ForEach(LoadProjectOptions.Add);
                projectDirectory = value;
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
    }
}