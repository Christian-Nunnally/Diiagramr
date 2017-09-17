using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using Diiagramr.Model;
using System.Windows.Forms;
using Diiagramr.View.CustomControls;
using StyletIoC;

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
            SerializeAndSave(project, ProjectDirectory + "\\" + project.Name);
            return true;
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

            if (_saveFileDialog.ShowDialog() != DialogResult.OK) return false;

            SerializeAndSave(project, _saveFileDialog.FileName);
            SetComponentsFromPath(project, _saveFileDialog.FileName);
            return true;
        }

        private void SerializeAndSave(Project project, string name)
        {
            project.PreSave();
            try
            {
                var serializer = new DataContractSerializer(typeof(Project));
                var settings = new XmlWriterSettings { Indent = true };
                using (var w = XmlWriter.Create(name, settings)) serializer.WriteObject(w, project);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void SetComponentsFromPath(Project project, string path)
        {
            var lastBackslashIndex = path.LastIndexOf("\\");
            if (lastBackslashIndex == -1) return;
            ProjectDirectory = path.Substring(0, lastBackslashIndex);
            var lastPeriod = path.LastIndexOf(".");
            project.Name = path.Substring(lastBackslashIndex + 1, lastPeriod - lastBackslashIndex - 1);
        }
    }
}
