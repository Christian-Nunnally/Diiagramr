namespace DiiagramrModel
{
    using PropertyChanged;
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;

    [DataContract]
    [AddINotifyPropertyChangedInterface]
    public class DiagramModel : ModelBase
    {
        public DiagramModel()
        {
            Name = string.Empty;
        }

        public virtual bool IsOpen { get; set; }

        public bool NameEditMode { get; set; } = false;

        public bool NotNameEditMode => !NameEditMode;

        [DataMember]
        public virtual ObservableCollection<NodeModel> Nodes { get; set; } = new ObservableCollection<NodeModel>();

        public virtual void AddNode(NodeModel nodeModel)
        {
            if (Nodes.Contains(nodeModel))
            {
                throw new ModelValidationException(this, "Remove this node from the diagram before trying to add it again.");
            }

            if (nodeModel.IsConnected)
            {
                throw new ModelValidationException(this, "Disconnect all wires from the node before adding it to a diagram.");
            }

            Nodes.Add(nodeModel);
        }

        public virtual void RemoveNode(NodeModel nodeModel)
        {
            if (!Nodes.Contains(nodeModel))
            {
                throw new ModelValidationException(this, "Add this node to the diagram before trying to remove it.");
            }

            if (nodeModel.IsConnected)
            {
                throw new ModelValidationException(this, "Disconnect all wires from the node before removing it from a diagram.");
            }

            Nodes.Remove(nodeModel);
        }

        public virtual void Open()
        {
            IsOpen = false;
            IsOpen = true;
        }
    }
}