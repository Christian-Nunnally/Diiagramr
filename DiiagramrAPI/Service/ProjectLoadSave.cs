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
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return (ProjectModel) _serializer.ReadObject(stream);
            }
        }

        public void Save(ProjectModel project, string fileName)
        {
            using (var writer = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite))
            {
                using (var w = XmlWriter.Create(writer))
                {
                    _serializer.WriteObject(w, project);
                }
            }
        }
    }
}