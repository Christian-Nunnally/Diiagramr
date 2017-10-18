using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Xml;
using DiiagramrAPI.Model;
using DiiagramrAPI.Service.Interfaces;

namespace DiiagramrIntegrationTest.IntegrationHelpers
{
    class TestLoadSave : IProjectLoadSave
    {
        private string _buffer;

        private MemoryStream _sw;

        private byte[] bytez;

        private DataContractSerializer _serializer;

        public TestLoadSave()
        {
            _serializer = new DataContractSerializer(typeof(ProjectModel));
        }
        public ProjectModel Open(string fileName)
        {
            var mStream = new MemoryStream(Encoding.UTF8.GetBytes(_buffer));
            using (var r = XmlTextReader.Create(mStream))
            {
                r.GetAttribute("ProjectModel");
                var project = (ProjectModel)_serializer.ReadObject(r);
                return project;
            }
            
        }

        public void Save(ProjectModel project, string name)
        {
            var settings = new XmlWriterSettings { Indent = true };
            _sw = new MemoryStream();
            using (var w = new XmlTextWriter(_sw, new UTF8Encoding(false))
                { Formatting = Formatting.Indented })
            {
                _serializer.WriteObject(w, project);
            }
            _buffer = Encoding.UTF8.GetString(_sw.ToArray());
            bytez = _sw.ToArray();
        }

        public override string ToString()
        {
            return _buffer;
        }
    }
}
