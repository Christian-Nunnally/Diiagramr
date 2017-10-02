using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Diiagramr.PluginNodeApi;

namespace Diiagramr.Model
{
    public enum TerminalKind
    {
        Input,
        Output
    }

    [DataContract(IsReference = true)]
    public class TerminalModel : ModelBase
    {
        protected TerminalModel()
        {
        }

        public TerminalModel(string name, Type type, Direction defaultDirection, TerminalKind kind, int index)
        {
            PropertyChanged += OnTerminalPropertyChanged;
            TerminalIndex = index;
            Direction = defaultDirection;
            Kind = kind;
            Type = type;
            Name = name;
        }

        /// <summary>
        ///     The index of the terminal. The first terminal added to a node gets index 0.
        /// </summary>
        [DataMember]
        public int TerminalIndex { get; set; }

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
        ///     Gets the overall x posiion of the terminal on the diagram.  NodeX + offsetX.
        /// </summary>
        [DataMember]
        public virtual double X { get; set; }

        /// <summary>
        ///     Gets the overall y posiion of the terminal on the diagram.  NodeY + offsetY.
        /// </summary>
        [DataMember]
        public virtual double Y { get; set; }

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

        [DataMember]
        public virtual Direction Direction { get; set; }

        [DataMember]
        public TerminalKind Kind { get; set; }

        /// <summary>
        ///     The wire that is connected to this terminal. Null if no wire is connected.
        /// </summary>
        [DataMember]
        public virtual WireModel ConnectedWire { get; set; }

        public Type Type { get; set; }

        [DataMember]
        public string TypeName
        {
            get => Type?.AssemblyQualifiedName;
            set => Type = Type.GetType(value);
        }

        [DataMember]
        public virtual string Name { get; set; }

        [DataMember]
        public virtual object Data { get; set; }

        public void OnTerminalPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(NodeX)) || e.PropertyName.Equals(nameof(OffsetX)))
                X = NodeX + OffsetX;
            else if (e.PropertyName.Equals(nameof(NodeY)) || e.PropertyName.Equals(nameof(OffsetY)))
                Y = NodeY + OffsetY;
            else if (e.PropertyName.Equals(nameof(ConnectedWire)))
                SemanticsChanged?.Invoke();
        }

        public virtual void NodePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var node = (NodeModel) sender;
            if (e.PropertyName.Equals(nameof(NodeModel.X)))
                NodeX = node.X;
            if (e.PropertyName.Equals(nameof(NodeModel.Y)))
                NodeY = node.Y;
        }

        public void AddToNode(NodeModel node)
        {
            node.PropertyChanged += NodePropertyChanged;
            NodeX = node.X;
            NodeY = node.Y;
        }

        public virtual void DisconnectWire()
        {
            ConnectedWire?.DisconnectWire();
        }

        public virtual void EnableWire()
        {
            ConnectedWire?.EnableWire();
        }

        public virtual void DisableWire()
        {
            ConnectedWire?.DisableWire();
        }

        public virtual void ResetWire()
        {
            ConnectedWire?.DisableWire();
            ConnectedWire?.ResetWire();
        }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
            PropertyChanged += OnTerminalPropertyChanged;
        }

        /// <summary>
        ///     Notifies listeners when the sematics of this terminal have changed.
        /// </summary>
        public virtual event Action SemanticsChanged;
    }
}