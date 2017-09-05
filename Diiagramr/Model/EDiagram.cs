using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Diiagramr.Model
{
    [DataContract]
    [AddINotifyPropertyChangedInterface]
    public class EDiagram : ModelBase
    {

        public EDiagram()
        {
            Nodes = new List<DiagramNode>();
            Name = "";
        }

        [DataMember]
        public bool IsOpen { get; set; }

        public bool IsExpanded => false;

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public List<DiagramNode> Nodes { get; set; }

        public void AddNode(DiagramNode diagramNode)
        {
            if (Nodes.Contains(diagramNode)) throw new InvalidOperationException("Can not add a diagramNode twice");
            Nodes.Add(diagramNode);
        }
    }
}