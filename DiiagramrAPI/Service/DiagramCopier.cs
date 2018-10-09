using DiiagramrAPI.Model;
using DiiagramrAPI.Service.Interfaces;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace DiiagramrAPI.Service
{
    public class DiagramCopier
    {
        private readonly IProjectManager _projectManager;
        private readonly DataContractSerializer _serializer;

        public DiagramCopier(IProjectManager projectManager)
        {
            _projectManager = projectManager;
            _serializer = new DataContractSerializer(typeof(DiagramModel), _projectManager.GetSerializeableTypes());
        }

        public DiagramModel Copy(DiagramModel diagram)
        {
            DiagramModel diagramCopy;
            var memoryStream = new MemoryStream();

            using (var xmlTextWriter = new XmlTextWriter(memoryStream, new UTF8Encoding(false)))
            {
                _serializer.WriteObject(xmlTextWriter, diagram);
            }

            var buffer = Encoding.UTF8.GetString(memoryStream.ToArray());
            memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(buffer));

            using (var xmlTextReader = XmlReader.Create(memoryStream))
            {
                diagramCopy = (DiagramModel)_serializer.ReadObject(xmlTextReader);
            }

            return diagramCopy;

            //var tempFileName = Path.GetTempFileName();
            //Save(diagram, tempFileName);
            //return Open(tempFileName);

        }

        public DiagramModel Open(string fileName)
        {
            var serializer = new DataContractSerializer(typeof(DiagramModel), _projectManager.GetSerializeableTypes());
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return (DiagramModel)serializer.ReadObject(stream);
            }
        }

        public void Save(DiagramModel diagram, string fileName)
        {
            var serializer = new DataContractSerializer(typeof(DiagramModel), _projectManager.GetSerializeableTypes());
            using (var writer = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite))
            {
                using (var w = XmlWriter.Create(writer))
                {
                    try
                    {
                        serializer.WriteObject(w, diagram);
                    }
                    catch (XmlException)
                    {
                    }
                }
            }
        }
    }
}
