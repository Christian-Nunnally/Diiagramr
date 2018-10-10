using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using PropertyChanged;

namespace DiiagramrAPI.Model
{
    [DataContract]
    [AddINotifyPropertyChangedInterface]
    public class DiagramModel : ModelBase
    {
        public DiagramModel()
        {
            DiagramName = "";
        }

        public virtual bool IsOpen { get; set; }

        public bool NameEditMode { get; set; } = false;

        public bool NotNameEditMode => !NameEditMode;

        [DataMember]
        public virtual string DiagramName { get; set; }

        [DataMember]
        public virtual List<NodeModel> Nodes { get; set; } = new List<NodeModel>();

        /// <summary>
        ///     Notifies listeners when the sematics of this diagram have changed.
        /// </summary>
        public event Action SemanticsChanged;

        /// <summary>
        ///     Notifies listeners when the appearance of this diagram have changed.
        /// </summary>
        public event Action PresentationChanged;

        public virtual void AddNode(NodeModel nodeModel)
        {
            if (Nodes.Contains(nodeModel)) throw new InvalidOperationException("Can not add a nodeModel twice");
            nodeModel.SemanticsChanged += NodeSematicsChanged;
            nodeModel.PresentationChanged += NodePresentationChanged;
            Nodes.Add(nodeModel);
            NodeSematicsChanged();
        }

        private void NodeSematicsChanged()
        {
            SemanticsChanged?.Invoke();
        }

        private void NodePresentationChanged()
        {
            PresentationChanged?.Invoke();
        }

        public virtual void RemoveNode(NodeModel nodeModel)
        {
            if (!Nodes.Contains(nodeModel)) throw new InvalidOperationException("Can not remove a nodeModel that isn't on the diagram");
            nodeModel.SemanticsChanged -= NodeSematicsChanged;
            nodeModel.PresentationChanged -= NodePresentationChanged;
            Nodes.Remove(nodeModel);
            NodeSematicsChanged();
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