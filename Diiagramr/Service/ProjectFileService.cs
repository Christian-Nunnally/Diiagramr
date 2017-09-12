using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using Diiagramr.Model;
using System.Windows.Forms;
using Diiagramr.View.CustomControls;
using StyletIoC;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace Diiagramr.Service
{
    public class ProjectFileService : IProjectFileService
    {
        public string ProjectDirectory { get; set; }

        private readonly IFileDialog _openFileDialog;

        private readonly IFileDialog _saveFileDialog;

        public ProjectFileService(IDirectoryService directoryService, [Inject(Key = "open")] IFileDialog openDialog, [Inject(Key = "save")] IFileDialog saveDialog)
        {
            _openFileDialog = openDialog;
            _saveFileDialog = saveDialog;
            ProjectDirectory = directoryService.GetCurrentDirectory() + "\\" + "Projects";

            if (!directoryService.Exists(ProjectDirectory)) directoryService.CreateDirectory(ProjectDirectory);
        }

        public bool SaveProject(Project project, bool saveAs)
        {
            if (saveAs || project.Name == "NewProject")
            {
                return SaveAsProject(project);
            }
            else
            {
                SerializeAndSave(project, ProjectDirectory + "\\" + project.Name);
                return true;
            }
        }

        public Project LoadProject()
        {
            _openFileDialog.InitialDirectory = ProjectDirectory;
            _openFileDialog.Filter = "Project files(*.xml)|*.xml|All files(*.*)|*.*";
            _openFileDialog.FileName = "";

            if (_openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var serializer = new DataContractSerializer(typeof(Project));
                Stream stream = new FileStream(_openFileDialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                var project = (Project)serializer.ReadObject(stream);
                stream.Close();
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

        private bool SaveAsProject(Project project)
        {
            if (project.Name != null)
            {
                _saveFileDialog.FileName = project.Name;
            }

            _saveFileDialog.InitialDirectory = ProjectDirectory;
            _saveFileDialog.Filter = "Project files(*.xml)|*.xml|All files(*.*)|*.*";

            if (_saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                SerializeAndSave(project, _saveFileDialog.FileName);
                SetComponentsFromPath(project, _saveFileDialog.FileName);
                return true;
            }

            return false;
        }

        private void SerializeAndSave(Project project, string name)
        {
            var serializer = new DataContractSerializer(typeof(Project), new Type[] { typeof(InputTerminal), typeof(OutputTerminal) });
            var settings = new XmlWriterSettings { Indent = true };
            using (var w = XmlWriter.Create(name, settings)) serializer.WriteObject(w, project);
        }

        private void SetComponentsFromPath(Project project, string path)
        {
            ProjectDirectory = path.Substring(0, path.LastIndexOf("\\"));
            project.Name = path.Substring(path.LastIndexOf("\\") + 1);
        }
    }
}
