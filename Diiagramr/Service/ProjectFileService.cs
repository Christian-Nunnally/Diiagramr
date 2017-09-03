using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using Diiagramr.Model;

namespace Diiagramr.Service
{
    public class ProjectFileService : IProjectFileService
    {
        public string ProjectDirectory { get; set; }

        private readonly IDirectoryService _directoryService;

        public ProjectFileService(IDirectoryService directoryService)
        {
            _directoryService = directoryService;
            ProjectDirectory = _directoryService.GetCurrentDirectory() + "\\" + "Projects";

            if (!_directoryService.Exists(ProjectDirectory)) _directoryService.CreateDirectory(ProjectDirectory);
        }

        public Project CreateProject(string name)
        {
            if (!IsProjectNameValid(name)) return null;

            _directoryService.CreateDirectory(ProjectDirectory + "\\" + name);
            return new Project(name);
        }

        public bool IsProjectNameValid(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            if (name.Length >= 2)
            {
                if (name[0] == ' ') return false;
                if (name[name.Length - 1] == ' ') return false;
            }
            return (!DoesProjectExist(name)) && name.All(IsValidDirectoryCharacter);
        }

        public void SaveProject(Project project)
        {
            var serializer = new DataContractSerializer(typeof(Project), new Type[] { typeof(InputTerminal), typeof(OutputTerminal)});
            var fileName = ProjectDirectory + "\\" + project.Name + "\\" + project.Name + ".xml";
            var settings = new XmlWriterSettings { Indent = true };
            using (var w = XmlWriter.Create(fileName, settings)) serializer.WriteObject(w, project);
        }

        public Project LoadProject(string projectName)
        {
            var serializer = new DataContractSerializer(typeof(Project));
            var fileName = ProjectDirectory + "\\" + projectName + "\\" + projectName + ".xml";
            Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            var project = (Project)serializer.ReadObject(stream);
            stream.Close();
            return project;
        }

        public IList<string> GetSavedProjectNames()
        {
            return _directoryService.GetDirectories(ProjectDirectory).ToList();
        }

        public bool MoveProject(string oldName, string newName)
        {
            if (!IsProjectNameValid(newName)) return false;
            if (!DoesProjectExist(oldName)) return false;
            _directoryService.Move(ProjectDirectory + "\\" + oldName, ProjectDirectory + "\\" + newName);
            _directoryService.Delete(ProjectDirectory + "\\" + oldName, true);
            return true;
        }

        private static bool IsValidDirectoryCharacter(char c)
        {
            return "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLNMOPQRSTUVWZYZ0123456789_- .".Contains(c.ToString());
        }

        private bool DoesProjectExist(string name)
        {
            return _directoryService.Exists(ProjectDirectory + "\\" + name);
        }
    }
}
