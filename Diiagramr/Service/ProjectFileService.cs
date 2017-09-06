using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using Diiagramr.Model;
using System.Windows.Forms;

namespace Diiagramr.Service
{
    public class ProjectFileService : IProjectFileService
    {
        public string ProjectDirectory { get; set; }

        public string ProjectName { get; set; }

        private readonly IDirectoryService _directoryService;

        public ProjectFileService(IDirectoryService directoryService)
        {
            _directoryService = directoryService;
            ProjectDirectory = _directoryService.GetCurrentDirectory() + "\\" + "Projects";

            if (!_directoryService.Exists(ProjectDirectory)) _directoryService.CreateDirectory(ProjectDirectory);
        }

        public bool SaveProject(Project project, bool saveAs)
        {
            if (saveAs || ProjectName == null)
            {
                return SaveAsProject(project);
            }
            else
            {
                SerializeAndSave(project, ProjectDirectory + "\\" + ProjectName);
                return true;
            }
        }

        public Project LoadProject()
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.InitialDirectory = ProjectDirectory;
            openFile.Filter = "Project files(*.xml)|*.xml|All files(*.*)|*.*";

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                var serializer = new DataContractSerializer(typeof(Project));
                Stream stream = new FileStream(openFile.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                var project = (Project)serializer.ReadObject(stream);
                stream.Close();
                SetPathComponents(openFile.FileName);
                return project;
            }

            return null;
        }

        public bool MoveProject(string oldName, string newName)
        {
            // TODO
            return false;
        }

        private bool SaveAsProject(Project project)
        {
            SaveFileDialog saveFile = new SaveFileDialog();

            if (ProjectName == null)
            {
                saveFile.FileName = project.Name;
            }
            else
            {
                saveFile.FileName = ProjectName;
            }

            saveFile.InitialDirectory = ProjectDirectory;
            saveFile.Filter = "Project files(*.xml)|*.xml|All files(*.*)|*.*";

            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                SerializeAndSave(project, saveFile.FileName);
                SetPathComponents(saveFile.FileName);
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

        private void SetPathComponents(string path)
        {
            ProjectDirectory = path.Substring(0, path.LastIndexOf("\\"));
            ProjectName = path.Substring(path.LastIndexOf("\\") + 1);
        }
    }
}
