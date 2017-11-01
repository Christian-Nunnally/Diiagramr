using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using DiiagramrAPI.Model;
using DiiagramrAPI.Service.Interfaces;

namespace DiiagramrAPI.Service
{
    public class ProjectLoadSave : IProjectLoadSave
    {
        private readonly DataContractSerializer _serializer;

        public ProjectLoadSave()
        {
            _serializer = new DataContractSerializer(typeof(ProjectModel));
        }

        public ProjectModel Open(string fileName)
        {
            Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            
            var project = (ProjectModel) _serializer.ReadObject(stream);
            stream.Close();
            return project;
        }

        public void Save(ProjectModel project, string name)
        {
            var settings = new XmlWriterSettings {Indent = true};
            using (var writer = new FileStream(name, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                using (var w = XmlWriter.Create(writer))
                {
                    _serializer.WriteObject(w, project);
                }
            }
        }
    }
}