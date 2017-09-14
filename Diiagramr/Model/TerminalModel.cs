using Diiagramr.Service;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Diiagramr.ViewModel.Diagram;

// ReSharper disable MemberCanBePrivate.Global Public setters used for deserialization

namespace Diiagramr.Model
{
    [DataContract(IsReference = true)]
    public class TerminalModel : ModelBase
    {
        /// <summary>
        /// The index of the terminal. The first terminal added to a node gets index 0.
        /// </summary>
        [DataMember]
        public int TerminalIndex { get; set; }

        /// <summary>
        /// The x position of the node this terminal belongs to.
        /// </summary>
        [DataMember]
        public double NodeX { get; set; }

        /// <summary>
        /// The y position of the node this terminal belongs to.
        /// </summary>
        [DataMember]
        public double NodeY { get; set; }

        /// <summary>
        /// Gets the overall x posiion of the terminal on the diagram.  NodeX + offsetX.
        /// </summary>
        [DataMember]
        public double X { get; set; }

        /// <summary>
        /// Gets the overall y posiion of the terminal on the diagram.  NodeY + offsetY.
        /// </summary>
        [DataMember]
        public double Y { get; set; }

        /// <summary>
        /// The x position of the terminal relative to the left of the node.
        /// </summary>
        [DataMember]
        public double OffsetX { get; set; }

        /// <summary>
        /// The y position of the terminal relative to the top of the node.
        /// </summary>
        [DataMember]
        public double OffsetY { get; set; }

        [DataMember]
        public Direction Direction { get; set; }

        /// <summary>
        /// The wire that is connected to this terminal. Null if no wire is connected.
        /// </summary>
        [DataMember]
        public Wire ConnectedWire { get; set; }

        public Type Type { get; set; }

        [DataMember]
        public string TypeName
        {
            get { return Type?.AssemblyQualifiedName; }
            set { Type = Type.GetType(value); }
        }

        [DataMember]
        public string Name { get; set; }

        public object Data { get; set; }

        protected TerminalModel() { }

        public TerminalModel(string name, Type type, int index)
        {
            TerminalIndex = index;
            Type = type;
            Name = name;
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

        public virtual void NodePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var node = (DiagramNode)sender;
            if (e.PropertyName.Equals("X"))
            {
                NodeX = node.X;
            }
            if (e.PropertyName.Equals("Y"))
            {
                NodeY = node.Y;
            }
        }

        public void DisconnectWire()
        {
            if (ConnectedWire == null) return;
            ConnectedWire.SourceTerminal = null;
            ConnectedWire.SinkTerminal = null;
            ConnectedWire = null;
        }

        /// <summary>
        /// Wiggles this instance so that property changed notifications are sent.
        /// </summary>
        public void Wiggle()
        {
            OnModelPropertyChanged(nameof(X));
            OnModelPropertyChanged(nameof(Y));
        }
    }
}