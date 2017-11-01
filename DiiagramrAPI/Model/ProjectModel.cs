using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using PropertyChanged;
using System.Collections.Generic;
using System;
using System.Diagnostics;

namespace DiiagramrAPI.Model
{
    [DataContract]
    [AddINotifyPropertyChangedInterface]
    public class ProjectModel : ModelBase
    {
        [DataMember]
        public virtual string Name { get; set; }

        [DataMember]
        public virtual ObservableCollection<DiagramModel> Diagrams { get; set; }

        public ProjectModel()
        {
            Diagrams = new ObservableCollection<DiagramModel>();
            Name = "NewProject";
        }
    }
}
