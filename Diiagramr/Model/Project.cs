using PropertyChanged;
using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Diiagramr.Service;

namespace Diiagramr.Model
{
    [DataContract]
    [KnownType(typeof(InputTerminal))]
    [KnownType(typeof(OutputTerminal))]
    [AddINotifyPropertyChangedInterface]
    public class Project : ModelBase
    {
        [XmlIgnore]
        public bool IsExpanded { get; set; }

        [DataMember]
        public virtual string Name { get; set; }

        [DataMember]
        public ObservableCollection<DiagramModel> Diagrams { get; set; }

        public Project()
        {
            Diagrams = new ObservableCollection<DiagramModel>();
            Name = "NewProject";
        }

        /// <summary>
        /// Must be called before the project is serialized and saved to disk.
        /// </summary>
        public virtual void PreSave()
        {
            Diagrams.ForEach(d => d.PreSave());
        }
    }
}
