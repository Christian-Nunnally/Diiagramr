using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using Diiagramr.Model;

namespace Diiagramr.Service
{
    public class DiagramCopier
    {
        private readonly DataContractSerializer _serializer;

        public DiagramCopier()
        {
            _serializer = new DataContractSerializer(typeof(DiagramModel));
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
        }
    }
}
