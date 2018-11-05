using DiiagramrAPI.Model;
using DiiagramrAPI.Service.Interfaces;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace DiiagramrAPI.Service
{
    public class DiagramCopier
    {
        private readonly IProjectManager _projectManager;
        private DataContractSerializer _serializer;

        public DiagramCopier(IProjectManager projectManager)
        {
            _projectManager = projectManager;
        }

        public DiagramModel Copy(DiagramModel diagram)
        {
            _serializer = new DataContractSerializer(typeof(DiagramModel), _projectManager.GetSerializeableTypes());
            DiagramModel diagramCopy;
            var memoryStream = new MemoryStream();

            var memoryStream2 = new NotifyingStream(s => { });

            using (var xmlTextWriter = XmlWriter.Create(memoryStream, new XmlWriterSettings { Encoding = Encoding.UTF8 }))
            {
                _serializer.WriteObject(xmlTextWriter, diagram);
            }
            memoryStream.Dispose();
            var buffer = Encoding.UTF8.GetString(memoryStream.ToArray());
            memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(buffer));

            using (var xmlTextReader = XmlReader.Create(memoryStream))
            {
                diagramCopy = (DiagramModel)_serializer.ReadObject(xmlTextReader);
            }

            memoryStream.Dispose();
            return diagramCopy;
        }
    }

    public class NotifyingStream : Stream
    {
        public override bool CanRead => false;

        public override bool CanSeek => false;

        public override bool CanWrite => true;

        public override long Length => 100;

        public override long Position { get => 0; set { } }

        private string _msg;
        public Action<string> Notify { get; }

        public NotifyingStream(Action<string> notify)
        {
            Notify = notify;
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return 0;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return 0;
        }

        public override void SetLength(long value)
        {
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _msg += Encoding.UTF8.GetString(buffer);
            Notify.Invoke(_msg);
        }
    }
}
