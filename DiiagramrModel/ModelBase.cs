namespace DiiagramrModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Xml;

    [DataContract(IsReference = true)]
    public class ModelBase : INotifyPropertyChanged
    {
        public static ISet<Type> SerializeableTypes = new HashSet<Type>();

        private string _name;

        public ModelBase()
        {
            Id = StaticId++;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public virtual string Name
        {
            get => _name;

            set
            {
                if (_name != value)
                {
                    NotifyPropertyChanged(nameof(Name));
                }

                _name = value;
            }
        }

        private static int StaticId { get; set; }

        public virtual ModelBase Copy()
        {
            var serializer = new DataContractSerializer(GetType(), SerializeableTypes);
            using var memoryStream = new MemoryStream();
            using (var xmlTextWriter = XmlWriter.Create(memoryStream, new XmlWriterSettings { Encoding = Encoding.UTF8 }))
            {
                serializer.WriteObject(xmlTextWriter, this);
            }
            memoryStream.Seek(0, SeekOrigin.Begin);
            using var xmlTextReader = XmlReader.Create(memoryStream);
            return (ModelBase)serializer.ReadObject(xmlTextReader);
        }

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}