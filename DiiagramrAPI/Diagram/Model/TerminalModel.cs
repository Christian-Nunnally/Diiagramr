using DiiagramrAPI.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace DiiagramrAPI.Diagram.Model
{
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

        /// <summary>
        /// Creates a new terminal model object.
        /// </summary>
        /// <param name="name">The user visible name of the terminal.</param>
        /// <param name="type">The data type of the terminal.</param>
        /// <param name="defaultSide">The default side of a node the terminal belongs on.</param>
        /// <param name="index">The unique index of the terminal.</param>
        public TerminalModel(string name, Type type, Direction defaultSide, int index)
        {
            TerminalIndex = index;
            DefaultSide = defaultSide;
            Type = type;
            Name = name;
        }

        /// <summary>
        /// The node that this terminal belongs to.
        /// </summary>
        [DataMember]
        public NodeModel ParentNode
        {
            get => _parentNode;
            set => ParentNode.UpdateListeningProperty(value, () => _parentNode = value, ParentNodePropertyChanged);
        }

        /// <summary>
        /// The list of wires connected to the terminal.
        /// </summary>
        [DataMember]
        public virtual List<WireModel> ConnectedWires { get; } = new List<WireModel>();

        /// <summary>
        /// The current value held by the terminal.
        /// </summary>
        [DataMember]
        public virtual object Data { get; set; }

        /// <summary>
        /// The side of the <see cref="ParentNode"/> the terminal is on.
        /// </summary>
        [DataMember]
        public virtual Direction DefaultSide { get; set; }

        /// <summary>
        /// The x position of the terminal relative to the left of the node.
        /// </summary>
        [DataMember]
        public virtual double OffsetX { get; set; }

        /// <summary>
        /// The y position of the terminal relative to the top of the node.
        /// </summary>
        [DataMember]
        public virtual double OffsetY { get; set; }

        /// <summary>
        /// The index of the terminal. The first terminal added to a node gets index 0.
        /// </summary>
        [DataMember]
        public int TerminalIndex { get; set; }

        // todo: These do no belong on the model
        /// <summary>
        /// The minimum distance the wire needs to extend down from this terminal to clear the parent node bounds in case the wire needs to route around the node.
        /// </summary>
        [DataMember]
        public double TerminalDownWireMinimumLength { get; set; }

        // todo: These do no belong on the model
        /// <summary>
        /// The minimum distance the wire needs to extend left from this terminal to clear the parent node bounds in case the wire needs to route around the node.
        /// </summary>
        [DataMember]
        public double TerminalLeftWireMinimumLength { get; set; }

        // todo: These do no belong on the model
        /// <summary>
        /// The minimum distance the wire needs to extend right from this terminal to clear the parent node bounds in case the wire needs to route around the node.
        /// </summary>
        [DataMember]
        public double TerminalRightWireMinimumLength { get; set; }

        // todo: These do no belong on the model
        /// <summary>
        /// The minimum distance the wire needs to extend up from this terminal to clear the parent node bounds in case the wire needs to route around the node.
        /// </summary>
        [DataMember]
        public double TerminalUpWireMinimumLength { get; set; }

        /// <summary>
        /// The data type of the terminal.
        /// </summary>
        [IgnoreDataMember]
        public Type Type { get; set; }

        /// <summary>
        /// The fully qualified name of <see cref="Type"/>.
        /// </summary>
        [DataMember]
        public string TypeName
        {
            get => Type?.AssemblyQualifiedName ?? _typeName;
            set => _typeName = value;
        }

        /// <summary>
        /// The overall x position of the terminal on the diagram.  NodeX + offsetX.
        /// </summary>
        [DataMember]
        public virtual double X => (ParentNode?.X ?? 0) + OffsetX;

        /// <summary>
        /// The overall y position of the terminal on the diagram.  NodeY + offsetY.
        /// </summary>
        [DataMember]
        public virtual double Y => (ParentNode?.Y ?? 0) + OffsetY;

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
            wire.SinkTerminal = null;
            wire.SourceTerminal = null;
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
