﻿using System;
using System.Windows.Forms;
using DiiagramrAPI.CustomControls;
using DiiagramrAPI.Model;
using DiiagramrAPI.Service.Interfaces;
using StyletIoC;

namespace DiiagramrAPI.Service
{
    public class ProjectFileService : IProjectFileService
    {
        private readonly IFileDialog _openFileDialog;

        private readonly IFileDialog _saveFileDialog;

        private readonly IProjectLoadSave _loadSave;

        public ProjectFileService(IDirectoryService directoryService, [Inject(Key = "open")] IFileDialog openDialog, [Inject(Key = "save")] IFileDialog saveDialog, IProjectLoadSave loadSave)
        {
            _openFileDialog = openDialog;
            _saveFileDialog = saveDialog;
            _loadSave = loadSave;
            ProjectDirectory = directoryService.GetCurrentDirectory() + "\\" + "Projects";

            if (!directoryService.Exists(ProjectDirectory)) directoryService.CreateDirectory(ProjectDirectory);
        }

        public string ProjectDirectory { get; set; }

        public bool SaveProject(ProjectModel project, bool saveAs)
        {
            if (saveAs || project.Name == "NewProject")
            {
                return SaveAsProject(project);
            }
            SerializeAndSave(project, ProjectDirectory + "\\" + project.Name + ".xml");
            return true;
        }

        public ProjectModel LoadProject()
        {
            _openFileDialog.InitialDirectory = ProjectDirectory;
            _openFileDialog.Filter = "ProjectModel files(*.xml)|*.xml|All files(*.*)|*.*";
            _openFileDialog.FileName = "";

            if (_openFileDialog.ShowDialog() != DialogResult.OK) return null;

            var project = _loadSave.Open(_openFileDialog.FileName);
            SetProjectNameFromPath(project, _openFileDialog.FileName);
            return project;
        }

        public DialogResult ConfirmProjectClose()
        {
            const string message = "Do you want to save before closing?";
            return MessageBox.Show(message, "Diiagramr", MessageBoxButtons.YesNoCancel);
        }

        private bool SaveAsProject(ProjectModel project)
        {
            if (project.Name != null)
            {
                _saveFileDialog.FileName = project.Name;
            }

            _saveFileDialog.InitialDirectory = ProjectDirectory;
            _saveFileDialog.Filter = "ProjectModel files(*.xml)|*.xml|All files(*.*)|*.*";

            if (_saveFileDialog.ShowDialog() != DialogResult.OK) return false;

            SerializeAndSave(project, _saveFileDialog.FileName);
            SetProjectNameFromPath(project, _saveFileDialog.FileName);
            return true;
        }

        private void SerializeAndSave(ProjectModel project, string name)
        {
            _loadSave.Save(project, name);
        }

        private void SetProjectNameFromPath(ProjectModel project, string path)
        {
            var lastBackslashIndex = path.LastIndexOf("\\", StringComparison.Ordinal);
            if (lastBackslashIndex == -1) return;
            ProjectDirectory = path.Substring(0, lastBackslashIndex);
            var lastPeriod = path.LastIndexOf(".", StringComparison.Ordinal);
            project.Name = path.Substring(lastBackslashIndex + 1, lastPeriod - lastBackslashIndex - 1);
        }
    }
}
