using PropertyChanged;
using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

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
        public string Name { get; set; }

        [DataMember]
        public ObservableCollection<EDiagram> Diagrams { get; set; }

        private Project() { }

        public Project(string name)
        {
            Diagrams = new ObservableCollection<EDiagram>();
            Name = name;
        }
    }
}
