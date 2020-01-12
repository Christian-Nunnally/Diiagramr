using DiiagramrAPI.Project;
using DiiagramrModel;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace DiiagramrAPI.Editor.Diagrams
{
    public class DiagramCopier
    {
        private readonly IProjectManager _projectManager;

        public DiagramCopier(IProjectManager projectManager)
        {
            _projectManager = projectManager;
        }

        public DiagramModel Copy(DiagramModel diagram)
        {
            var serializer = new DataContractSerializer(typeof(DiagramModel), _projectManager.GetSerializeableTypes());
            using (var memoryStream = new MemoryStream())
            {
                using (var xmlTextWriter = XmlWriter.Create(memoryStream, new XmlWriterSettings { Encoding = Encoding.UTF8 }))
                {
                    serializer.WriteObject(xmlTextWriter, diagram);
                }

                memoryStream.Seek(0, SeekOrigin.Begin);

                using (var xmlTextReader = XmlReader.Create(memoryStream))
                {
                    return (DiagramModel)serializer.ReadObject(xmlTextReader);
                }
            }
        }
    }
}