namespace DiiagramrModel
{
    using DiiagramrCore;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    /// <summary>
    /// A terminal provides nodes with a mechanism to share data with the diagram,
    /// is does this by maintaining a <see cref="Data"/> field that nodes can internally
    /// interface with. Clients can get notified when the <see cref="Data"/> in the terminal.
    /// </summary>
    [DataContract(IsReference = true)]
    public class TerminalModel : ModelBase
    {
        private string _typeName;
        private NodeModel _parentNode;
        private object data;

        /// <summary>
        /// Initializes a new instance of the <see cref="TerminalModel"/> class.
        /// </summary>
        /// <param name="name">The user visible name of the terminal.</param>
        /// <param name="type">The data type of the terminal.</param>
        /// <param name="defaultSide">The default side of a node the terminal belongs on.</param>
        public TerminalModel(string name, Type type, Direction defaultSide)
        {
            DefaultSide = defaultSide;
            Type = type;
            Name = name;
        }

        public event Action<object> DataChanged;

        /// <summary>
        /// Gets or sets the node that this terminal belongs to.
        /// </summary>
        [DataMember]
        public NodeModel ParentNode
        {
            get => _parentNode;
            set => ParentNode.UpdateListeningProperty(value, () => _parentNode = value, ParentNodePropertyChanged);
        }

        /// <summary>
        /// Gets the list of wires connected to the terminal.
        /// </summary>
        [DataMember]
        public virtual List<WireModel> ConnectedWires { get; set; } = new List<WireModel>();

        /// <summary>
        /// Gets or sets the current value held by the terminal.
        /// </summary>
        [DataMember]
        public virtual object Data
        {
            get => data;
            set
            {
                data = value;
                DataChanged?.Invoke(data);
            }
        }

        /// <summary>
        /// Gets or sets the side of the <see cref="ParentNode"/> the terminal is on.
        /// </summary>
        [DataMember]
        public virtual Direction DefaultSide { get; set; }

        /// <summary>
        /// Gets or sets the x position of the terminal relative to the left of the node.
        /// </summary>
        [DataMember]
        public virtual double OffsetX { get; set; }

        /// <summary>
        /// Gets or sets the y position of the terminal relative to the top of the node.
        /// </summary>
        [DataMember]
        public virtual double OffsetY { get; set; }

        /// <summary>
        /// Gets or sets the index of the terminal. The first terminal added to a node gets index 0.
        /// </summary>
        [DataMember]
        public int TerminalIndex { get; set; }

        /// <summary>
        /// Gets or sets the data type of the terminal.
        /// </summary>
        [IgnoreDataMember]
        public Type Type { get; set; }

        /// <summary>
        /// Gets the data type of the data currently on the terminal.
        /// </summary>
        [IgnoreDataMember]
        public Type CurrentType => Data?.GetType() ?? Type;

        /// <summary>
        /// Gets or sets the fully qualified name of <see cref="Type"/>.
        /// </summary>
        [DataMember]
        public string TypeName
        {
            get => Type?.AssemblyQualifiedName ?? _typeName;
            set => _typeName = value;
        }

        /// <summary>
        /// Gets the overall x position of the terminal on the diagram.  NodeX + offsetX.
        /// </summary>
        public virtual double X => (ParentNode?.X ?? 0) + OffsetX;

        /// <summary>
        /// Gets the overall y position of the terminal on the diagram.  NodeY + offsetY.
        /// </summary>
        public virtual double Y => (ParentNode?.Y ?? 0) + OffsetY;

        /// <summary>
        /// Gets or sets the edge of ther terminal along the edge of the node its on.
        /// </summary>
        public int EdgeIndex { get; set; }

        /// <summary>
        /// Connects to another terminal via a wire.
        /// </summary>
        /// <param name="wire">The wire to connect the terminals with.</param>
        /// <param name="otherTerminal">The terminal to wire to.</param>
        public virtual void ConnectWire(WireModel wire, TerminalModel otherTerminal)
        {
            if (ConnectedWires.Contains(wire))
            {
                throw new ModelValidationException(this, "Remove this wire from a terminal before connecting it again");
            }

            if (!CanWireToType(otherTerminal.Type))
            {
                throw new ModelValidationException(this, "Only connect wires between terminals with compatable types");
            }

            wire.SinkTerminal = this;
            wire.SourceTerminal = otherTerminal;
            otherTerminal.ConnectedWires.Add(wire);
            ConnectedWires.Add(wire);
        }

        /// <summary>
        /// Disconnects a wire connected to another terminal.
        /// </summary>
        /// <param name="wire">The wire disconnect.</param>
        /// <param name="otherTerminal">The terminal to disconnect from.</param>
        public virtual void DisconnectWire(WireModel wire, TerminalModel otherTerminal)
        {
            if (!ConnectedWires.Contains(wire) || !otherTerminal.ConnectedWires.Contains(wire))
            {
                throw new ModelValidationException(this, "Wire must be connected in order to disconnect it");
            }

            otherTerminal.ConnectedWires.Remove(wire);
            ConnectedWires.Remove(wire);
            if (wire.SinkTerminal.ConnectedWires.Count == 0)
            {
                wire.SinkTerminal.Data = null;
            }
            wire.SinkTerminal = null;
            wire.SourceTerminal = null;
        }

        public virtual bool CanWireToType(Type type)
        {
            return CanWireDataToType(Data, type);
        }

        public virtual bool CanWireFromData(object data)
        {
            return CanWireDataToType(data, Data?.GetType() ?? Type);
        }

        private bool CanWireDataToType(object data, Type to)
        {
            if (data == null)
            {
                return true;
            }
            return ValueConverter.TryCoerseValue(data, to, out _);
        }

        private void ParentNodePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(NodeModel.X))
            {
                NotifyPropertyChanged(nameof(X));
            }
            else if (e.PropertyName == nameof(NodeModel.Y))
            {
                NotifyPropertyChanged(nameof(Y));
            }
        }
    }
}