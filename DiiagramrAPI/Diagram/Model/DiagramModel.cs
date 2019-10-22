using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DiiagramrAPI.Diagram.Model
{
    [DataContract]
    [AddINotifyPropertyChangedInterface]
    public class DiagramModel : ModelBase
    {
        public DiagramModel()
        {
            Name = "";
        }

        public virtual bool IsOpen { get; set; }

        public bool NameEditMode { get; set; } = false;

        [DataMember]
        public virtual List<NodeModel> Nodes { get; set; } = new List<NodeModel>();

        public bool NotNameEditMode => !NameEditMode;

        public virtual void AddNode(NodeModel nodeModel)
        {
            if (Nodes.Contains(nodeModel))
            {
                throw new InvalidOperationException("Can not add a node twice.");
            }

            Nodes.Add(nodeModel);
        }

        public virtual void Pause()
        {
            Nodes.ForEach(n => n.DisableTerminals());
        }

        public virtual void Play()
        {
            Nodes.ForEach(n => n.EnableTerminals());
        }

        public virtual void Open()
        {
            IsOpen = false;
            IsOpen = true;
        }

        public virtual void RemoveNode(NodeModel nodeModel)
        {
            if (Nodes.Contains(nodeModel))
            {
                Nodes.Remove(nodeModel);
            }
        }

        public virtual void Stop()
        {
            Nodes.ForEach(n => n.ResetTerminals());
        }
    }
}
