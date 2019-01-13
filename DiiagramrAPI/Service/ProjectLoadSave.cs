using DiiagramrAPI.Model;
using DiiagramrAPI.Service.Interfaces;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace DiiagramrAPI.Service
{
    public class ProjectLoadSave : IProjectLoadSave
    {
        private readonly IPluginLoader _pluginLoader;

        public ProjectLoadSave(Func<IPluginLoader> pluginLoaderFactory)
        {
            _pluginLoader = pluginLoaderFactory.Invoke();
        }

        public ProjectModel Open(string fileName)
        {
            var serializer = new DataContractSerializer(typeof(ProjectModel), _pluginLoader.SerializeableTypes);
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return (ProjectModel)serializer.ReadObject(stream);
            }
        }

        public void Save(ProjectModel project, string fileName)
        {
            var serializer = new DataContractSerializer(typeof(ProjectModel), _pluginLoader.SerializeableTypes);
            using (var writer = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite))
            {
                using (var w = XmlWriter.Create(writer))
                {
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
    }
}
