using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using PropertyChanged;

namespace Diiagramr.Model
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

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public virtual List<NodeModel> Nodes { get; set; } = new List<NodeModel>();

        /// <summary>
        ///     Notifies listeners when the sematics of this diagram have changed.
        /// </summary>
        public event Action SemanticsChanged;

        public virtual void AddNode(NodeModel nodeModel)
        {
            if (Nodes.Contains(nodeModel)) throw new InvalidOperationException("Can not add a nodeModel twice");
            nodeModel.SemanticsChanged += NodeSematicsChanged;
            Nodes.Add(nodeModel);
            SemanticsChanged?.Invoke();
        }

        private void NodeSematicsChanged()
        {
            SemanticsChanged?.Invoke();
        }

        public virtual void RemoveNode(NodeModel nodeModel)
        {
            if (!Nodes.Contains(nodeModel)) throw new InvalidOperationException("Can not remove a nodeModel that isn't on the diagram");
            nodeModel.SemanticsChanged -= NodeSematicsChanged;
            Nodes.Remove(nodeModel);
            SemanticsChanged?.Invoke();
        }

        public virtual void Play()
        {
            Nodes.ForEach(n => n.EnableTerminals());
        }

        public virtual void Pause()
        {
            Nodes.ForEach(n => n.DisableTerminals());
        }

        public virtual void Stop()
        {
            Nodes.ForEach(n => n.ResetTerminals());
        }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
            Nodes.ForEach(n => n.SemanticsChanged += NodeSematicsChanged);
        }
    }
}