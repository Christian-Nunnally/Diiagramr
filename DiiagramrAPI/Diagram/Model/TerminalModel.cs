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

        public TerminalModel(string name, Type type, Direction defaultDirection, TerminalKind kind, int index)
        {
            ConnectedWires = new List<WireModel>();
            PropertyChanged += OnTerminalPropertyChanged;
            TerminalIndex = index;
            Direction = defaultDirection;
            Kind = kind;
            Type = type;
            Name = name;
        }

        protected TerminalModel()
        {
        }

        /// <summary>
        ///     Notifies listeners when the sematics of this terminal have changed.
        /// </summary>
        public virtual event Action SemanticsChanged;

        [DataMember]
        public virtual List<WireModel> ConnectedWires { get; set; }

        [DataMember]
        public virtual object Data { get; set; }

        [DataMember]
        public virtual Direction Direction { get; set; }

        public int EdgeIndex { get; set; }

        [DataMember]
        public TerminalKind Kind { get; set; }

        /// <summary>
        ///     The x position of the node this terminal belongs to.
        /// </summary>
        [DataMember]
        public double NodeX { get; set; }

        /// <summary>
        ///     The y position of the node this terminal belongs to.
        /// </summary>
        [DataMember]
        public double NodeY { get; set; }

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
        public virtual double X { get; set; }

        /// <summary>
        ///     Gets the overall y position of the terminal on the diagram.  NodeY + offsetY.
        /// </summary>
        [DataMember]
        public virtual double Y { get; set; }

        public void AddToNode(NodeModel node)
        {
            node.PropertyChanged += NodePropertyChanged;
            NodeX = node.X;
            NodeY = node.Y;
        }

        public virtual void ConnectWire(WireModel wire)
        {
            if (ConnectedWires.Contains(wire))
            {
                return;
            }

            ConnectedWires.Add(wire);
            SemanticsChanged?.Invoke();
        }

        public virtual void DisableWire()
        {
            foreach (var connectedWire in ConnectedWires)
            {
                connectedWire.DisableWire();
            }
        }

        public virtual void DisconnectWire(WireModel wire)
        {
            ConnectedWires.Remove(wire);
            SemanticsChanged?.Invoke();
        }

        public virtual void DisconnectWires()
        {
            for (var i = ConnectedWires.Count - 1; i >= 0; i--)
            {
                ConnectedWires[i].DisconnectWire();
            }
        }

        public virtual void EnableWire()
        {
            foreach (var connectedWire in ConnectedWires)
            {
                connectedWire.EnableWire();
            }
        }

        public virtual void NodePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var node = (NodeModel)sender;
            if (e.PropertyName.Equals(nameof(NodeModel.X)))
            {
                NodeX = node.X;
            }

            if (e.PropertyName.Equals(nameof(NodeModel.Y)))
            {
                NodeY = node.Y;
            }
        }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
            PropertyChanged += OnTerminalPropertyChanged;
        }

        public void OnTerminalPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(NodeX)) || e.PropertyName.Equals(nameof(OffsetX)))
            {
                X = NodeX + OffsetX;
            }
            else if (e.PropertyName.Equals(nameof(NodeY)) || e.PropertyName.Equals(nameof(OffsetY)))
            {
                Y = NodeY + OffsetY;
            }
        }

        public virtual void ResetWire()
        {
            foreach (var connectedWire in ConnectedWires)
            {
                connectedWire.DisableWire();
                connectedWire.ResetWire();
            }
        }
    }
}
