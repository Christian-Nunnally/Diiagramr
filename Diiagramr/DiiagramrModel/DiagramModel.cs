namespace DiiagramrModel
{
    using PropertyChanged;
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;

    /// <summary>
    /// The model object for a diagram.
    /// </summary>
    [DataContract]
    [AddINotifyPropertyChangedInterface]
    public class DiagramModel : ModelBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="DiagramModel"/>.
        /// </summary>
        public DiagramModel()
        {
            Nodes = new ObservableCollection<NodeModel>();
            Name = string.Empty;
        }

        /// <summary>
        /// The visible list of node models contained by this diagram.
        /// </summary>
        [DataMember]
        public virtual ObservableCollection<NodeModel> Nodes { get; set; }

        /// <summary>
        /// Adds a node to the diagram.
        /// </summary>
        /// <param name="nodeModel">The node to add.</param>
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

        /// <summary>
        /// Removes a node from the diagram.
        /// </summary>
        /// <param name="nodeModel">The node to remove.</param>
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
    }
}