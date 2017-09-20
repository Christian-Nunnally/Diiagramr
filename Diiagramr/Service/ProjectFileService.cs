using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using Diiagramr.Model;
using System.Windows.Forms;
using Diiagramr.Service.Interfaces;
using Diiagramr.View.CustomControls;
using StyletIoC;

namespace Diiagramr.Service
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
            SerializeAndSave(project, ProjectDirectory + "\\" + project.Name);
            return true;
        }

        public ProjectModel LoadProject()
        {
            _openFileDialog.InitialDirectory = ProjectDirectory;
            _openFileDialog.Filter = "ProjectModel files(*.xml)|*.xml|All files(*.*)|*.*";
            _openFileDialog.FileName = "";

            if (_openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var project = _loadSave.Open(_openFileDialog.FileName);
                SetComponentsFromPath(project, _openFileDialog.FileName);
                return project;
            }

            return null;
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
            SetComponentsFromPath(project, _saveFileDialog.FileName);
            return true;
        }

        private void SerializeAndSave(ProjectModel project, string name)
        {
            _loadSave.Save(project, name);
        }

        private void SetComponentsFromPath(ProjectModel project, string path)
        {
            var lastBackslashIndex = path.LastIndexOf("\\");
            if (lastBackslashIndex == -1) return;
            ProjectDirectory = path.Substring(0, lastBackslashIndex);
            var lastPeriod = path.LastIndexOf(".");
            project.Name = path.Substring(lastBackslashIndex + 1, lastPeriod - lastBackslashIndex - 1);
        }
    }
}
