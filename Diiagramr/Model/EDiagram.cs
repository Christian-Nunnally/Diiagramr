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
            Name = "";
        }

        public virtual bool IsOpen { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public virtual List<DiagramNode> Nodes { get; set; } = new List<DiagramNode>();

        public virtual void AddNode(DiagramNode diagramNode)
        {
            if (Nodes.Contains(diagramNode)) throw new InvalidOperationException("Can not add a diagramNode twice");
            Nodes.Add(diagramNode);
        }
    }
}