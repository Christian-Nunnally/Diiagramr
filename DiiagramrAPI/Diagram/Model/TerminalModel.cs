using DiiagramrAPI.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace DiiagramrAPI.Diagram.Model
{
    public enum TerminalKind
    {
        Input,
        Output
    }

    [DataContract(IsReference = true)]
    public class TerminalModel : ModelBase
    {
        private string _typeName;
        private NodeModel _parentNode;

        public TerminalModel(string name, Type type, Direction defaultDirection, TerminalKind kind, int index)
        {
            ConnectedWires = new List<WireModel>();
            TerminalIndex = index;
            Direction = defaultDirection;
            Kind = kind;
            Type = type;
            Name = name;
            PropertyChanged += PropertyChangedHandler;
        }

        private void PropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            if (IsOutput && e.PropertyName == nameof(Data))
            {
                foreach (var wire in ConnectedWires)
                {
                    wire.SinkTerminal.Data = Data;
                }
            }
        }

        protected TerminalModel()
        {
        }

        [DataMember]
        public NodeModel ParentNode
        {
            get => _parentNode;
            set => ParentNode.UpdateListeningProperty(value, () => _parentNode = value, ParentNodePropertyChanged);
        }

        [DataMember]
        public virtual List<WireModel> ConnectedWires { get; set; }

        [DataMember]
        public virtual object Data { get; set; }

        [DataMember]
        public virtual Direction Direction { get; set; }

        public int EdgeIndex { get; set; }

        [DataMember]
        public TerminalKind Kind { get; set; }

        public bool IsInput => Kind == TerminalKind.Input;
        public bool IsOutput => Kind == TerminalKind.Output;

        /// <summary>
        ///     The x position of the terminal relative to the left of the node.
        /// </summary>
        [DataMember]
        public virtual double OffsetX { get; set; }

        /// <summary>
        ///     The y position of the terminal relative to the top of the node.
        /// </summary>
        [DataMember]
        public virtual double OffsetY { get; set; }

        /// <summary>
        ///     The index of the terminal. The first terminal added to a node gets index 0.
        /// </summary>
        [DataMember]
        public int TerminalIndex { get; set; }

        [DataMember]
        public double TerminalDownWireMinimumLength { get; set; }

        [DataMember]
        public double TerminalLeftWireMinimumLength { get; set; }

        [DataMember]
        public double TerminalRightWireMinimumLength { get; set; }

        [DataMember]
        public double TerminalUpWireMinimumLength { get; set; }

        [IgnoreDataMember]
        public Type Type { get; set; }

        [DataMember]
        public string TypeName
        {
            get => Type?.AssemblyQualifiedName ?? _typeName;
            set => _typeName = value;
        }

        /// <summary>
        ///     Gets the overall x position of the terminal on the diagram.  NodeX + offsetX.
        /// </summary>
        [DataMember]
        public virtual double X => (ParentNode?.X ?? 0) + OffsetX;

        /// <summary>
        ///     Gets the overall y position of the terminal on the diagram.  NodeY + offsetY.
        /// </summary>
        [DataMember]
        public virtual double Y => (ParentNode?.Y ?? 0) + OffsetY;

        public virtual void ConnectWire(WireModel wire, TerminalModel otherTerminal)
        {
            if (ConnectedWires.Contains(wire))
            {
                throw new ModelValidationException(this, "Remove this wire from a terminal before connecting it again");
            }
            if (otherTerminal.Kind == Kind)
            {
                throw new ModelValidationException(this, "Only input terminals to output terminals");
            }

            if (IsInput)
            {
                wire.SinkTerminal = this;
                wire.SourceTerminal = otherTerminal;
                
            }
            else
            {
                wire.SinkTerminal = otherTerminal;
                wire.SourceTerminal = this;
            }
            otherTerminal.ConnectedWires.Add(wire);
            ConnectedWires.Add(wire);
        }

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

        public virtual void ResetWire()
        {
            foreach (var wire in ConnectedWires)
            {
                wire.ResetWire();
            }
        }
    }
}
