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

        /// <summary>
        ///     Notifies listeners when the appearance of this diagram have changed.
        /// </summary>
        public event Action PresentationChanged;

        /// <summary>
        ///     Notifies listeners when the sematics of this diagram have changed.
        /// </summary>
        public event Action SemanticsChanged;

        public virtual bool IsOpen { get; set; }

        public bool NameEditMode { get; set; } = false;

        [DataMember]
        public virtual List<NodeModel> Nodes { get; set; } = new List<NodeModel>();

        public bool NotNameEditMode => !NameEditMode;

        public virtual void AddNode(NodeModel nodeModel)
        {
            if (Nodes.Contains(nodeModel))
            {
                throw new InvalidOperationException("Can not add a nodeModel twice");
            }

            nodeModel.SemanticsChanged += NodeSematicsChanged;
            nodeModel.PresentationChanged += NodePresentationChanged;
            Nodes.Add(nodeModel);
            NodeSematicsChanged();
        }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
            Nodes.ForEach(n => n.SemanticsChanged += NodeSematicsChanged);
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
            if (!Nodes.Contains(nodeModel))
            {
                return;
            }

            nodeModel.SemanticsChanged -= NodeSematicsChanged;
            nodeModel.PresentationChanged -= NodePresentationChanged;
            Nodes.Remove(nodeModel);
            NodeSematicsChanged();
        }

        public virtual void Stop()
        {
            Nodes.ForEach(n => n.ResetTerminals());
        }

        private void NodePresentationChanged()
        {
            PresentationChanged?.Invoke();
        }

        private void NodeSematicsChanged()
        {
            SemanticsChanged?.Invoke();
        }
    }
}
