using DiiagramrAPI.Service.Plugins;
using DiiagramrModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;

namespace DiiagramrAPI.Project
{
    public class ProjectLoadSave : IProjectLoadSave
    {
        private readonly IPluginLoader _pluginLoader;

        public ProjectLoadSave(Func<IPluginLoader> pluginLoaderFactory)
        {
            _pluginLoader = pluginLoaderFactory.Invoke();
        }

        public ProjectModel Open(string fullPath)
        {
            var serializer = new DataContractSerializer(typeof(ProjectModel), GetSerializableTypes());
            using (var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return (ProjectModel)serializer.ReadObject(stream);
            }
        }

        public void Save(ProjectModel project, string fullPath)
        {
            var serializer = new DataContractSerializer(typeof(ProjectModel), GetSerializableTypes());
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

        private IEnumerable<Type> GetSerializableTypes()
        {
            return _pluginLoader.SerializeableTypes.Concat(new[] { typeof(InputTerminalModel), typeof(OutputTerminalModel) });
        }
    }
}