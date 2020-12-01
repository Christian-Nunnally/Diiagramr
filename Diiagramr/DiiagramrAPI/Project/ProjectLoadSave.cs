using DiiagramrModel;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace DiiagramrAPI.Project
{
    /// <summary>
    /// Serializes and deserializes projects to/from disk.
    /// </summary>
    public class ProjectLoadSave : IProjectLoadSave
    {
        /// <summary>
        /// Creates a new instance of <see cref="ProjectLoadSave"/>.
        /// </summary>
        public ProjectLoadSave()
        {
        }

        /// <inheritdoc/>
        public ProjectModel Load(string fullPath)
        {
            var serializer = new DataContractSerializer(typeof(ProjectModel), ModelBase.SerializeableTypes);
            using var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return (ProjectModel)serializer.ReadObject(stream);
        }

        /// <inheritdoc/>
        public void Save(ProjectModel project, string fullPath)
        {
            project.IsDirty = false;
            foreach (var diagram in project.Diagrams)
            {
                foreach (var node in diagram.Nodes)
                {
                    foreach (var terminal in node.Terminals)
                    {
                        ModelBase.SerializeableTypes.Add(terminal.Type);
                    }
                }
            }

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