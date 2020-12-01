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

    /// <summary>
    /// The base class for all model objects in the application.
    /// </summary>
    [DataContract(IsReference = true)]
    public abstract class ModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// A global list of serializable types that should be known to the serializer.
        /// </summary>
        public static ISet<Type> SerializeableTypes = new HashSet<Type>();

        private string _name;

        /// <summary>
        /// Creates a new instance of <see cref="ModelBase"/>.
        /// </summary>
        public ModelBase()
        {
            Id = StaticId++;
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The unique ID of this model.
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The name of this model.
        /// </summary>
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

        /// <summary>
        /// Creates a carbon copy of this model by serializing it and deserializing it.
        /// </summary>
        /// <returns>The copy of this model.</returns>
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

        /// <inheritdoc/>
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}