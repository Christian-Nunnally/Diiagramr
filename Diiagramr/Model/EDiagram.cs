using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using PropertyChanged;

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