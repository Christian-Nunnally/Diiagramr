using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Diiagramr.Model;
using Diiagramr.Service.Interfaces;

namespace Diiagramr.Service
{
    public class ProjectLoadSave : IProjectLoadSave
    {

        private DataContractSerializer _serializer;

        public ProjectLoadSave()
        {
            _serializer = new DataContractSerializer(typeof(ProjectModel));
        }

        public ProjectModel Open(string name)
        {
            Stream stream = new FileStream(name, FileMode.Open, FileAccess.Read, FileShare.Read);
            var project = (ProjectModel) _serializer.ReadObject(stream);
            stream.Close();
            return project;
        }

        public void Save(ProjectModel project, string name)
        {
            var settings = new XmlWriterSettings { Indent = true };
            using (var w = XmlWriter.Create(name, settings)) _serializer.WriteObject(w, project);
        }
    }
}
