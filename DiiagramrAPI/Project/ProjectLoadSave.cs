using DiiagramrModel;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace DiiagramrAPI.Project
{
    public class ProjectLoadSave : IProjectLoadSave
    {
        public ProjectLoadSave()
        {
        }

        public ProjectModel Open(string fullPath)
        {
            var serializer = new DataContractSerializer(typeof(ProjectModel), ModelBase.SerializeableTypes);
            using var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return (ProjectModel)serializer.ReadObject(stream);
        }

        public void Save(ProjectModel project, string fullPath)
        {
            var serializer = new DataContractSerializer(typeof(ProjectModel), ModelBase.SerializeableTypes);
            using var writer = new FileStream(fullPath, FileMode.Create, FileAccess.ReadWrite);
            using var w = XmlWriter.Create(writer);
            try
            {
                serializer.WriteObject(w, project);
            }
            catch (XmlException e)
            {
                Console.WriteLine(e.InnerException.Message);
            }
        }
    }
}