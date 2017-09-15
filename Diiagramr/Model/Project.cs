﻿using PropertyChanged;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Diiagramr.Service;

namespace Diiagramr.Model
{
    [DataContract]
    [AddINotifyPropertyChangedInterface]
    public class Project : ModelBase
    {
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
