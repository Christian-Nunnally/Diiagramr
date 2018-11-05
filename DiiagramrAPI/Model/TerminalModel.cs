﻿using DiiagramrAPI.PluginNodeApi;
using DiiagramrAPI.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;

namespace DiiagramrAPI.Model
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
            ConnectedWires = new List<WireModel>();
            PropertyChanged += OnTerminalPropertyChanged;
            TerminalIndex = index;
            Direction = defaultDirection;
            Kind = kind;
            Type = type;
            Name = name;
        }

        public Action<WireModel> WireConnected;
        public Action<WireModel> WireDisconnected;

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

        [DataMember]
        public string MethodKey { get; set; }

        public bool ShouldSerializeData { get; set; } = true;

        /// <summary>
        ///     The wire that is connected to this terminal. Null if no wire is connected.
        /// </summary>
        [DataMember]
        public virtual List<WireModel> ConnectedWires { get; set; }

        [IgnoreDataMember]
        public Type Type { get; set; }

        private string _typeName;

        [DataMember]
        public string TypeName
        {
            get => GetSafeTypeName(Type?.AssemblyQualifiedName ?? _typeName);
            set => _typeName = GetRealTypeName(value);
        }

        private string GetRealTypeName(string typeName)
        {
            if (typeName == null)
            {
                return null;
            }

            var typeNameInvalidChars = new string[] { ".", " ", "=", "," };
            var typeNameValidChars = new string[] { "dddottt", "ssspaceee", "eeequalsss", "cccomaaa" };

            var realName = typeName;
            for (int i = 0; i < typeNameInvalidChars.Length; i++)
            {
                realName = realName.Replace(typeNameValidChars[i], typeNameInvalidChars[i]);
            }
            return realName;
        }

        private string GetSafeTypeName(string typeName)
        {
            if (typeName == null)
            {
                return null;
            }

            var typeNameInvalidChars = new string[] { ".", " ", "=", "," };
            var typeNameValidChars = new string[] { "dddottt", "ssspaceee", "eeequalsss", "cccomaaa" };

            var safeName = typeName;
            for (int i = typeNameInvalidChars.Length - 1; i >= 0; i--)
            {
                safeName = safeName.Replace(typeNameInvalidChars[i], typeNameValidChars[i]);
            }
            return safeName;
        }

        [DataMember]
        public virtual string Name { get; set; }

        [IgnoreDataMember]
        public virtual object Data { get; set; }

        [DataMember]
        public virtual object SerializedData
        {
            get => ShouldSerializeData ? Data : null;
            set => Data = ShouldSerializeData ? value : null;
        }

        public double TerminalUpWireMinimumLength { get; set; }
        public double TerminalDownWireMinimumLength { get; set; }
        public double TerminalLeftWireMinimumLength { get; set; }
        public double TerminalRightWireMinimumLength { get; set; }
        public int EdgeIndex { get; set; }

        private Type TypeResolver(Assembly assembly, string name, bool ignore)
        {
            return assembly == null ? Type.GetType(name, false, ignore) : assembly.GetType(name, false, ignore);
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

        public void AddToNode(NodeModel node)
        {
            node.PropertyChanged += NodePropertyChanged;
            NodeX = node.X;
            NodeY = node.Y;
        }

        public virtual void DisconnectWires()
        {
            for (var i = ConnectedWires.Count - 1; i >= 0; i--)
            {
                ConnectedWires[i].DisconnectWire();
            }
        }

        public virtual void DisconnectWire(WireModel wire)
        {
            ConnectedWires.Remove(wire);
            WireDisconnected?.Invoke(wire);
            SemanticsChanged?.Invoke();
        }

        public virtual void ConnectWire(WireModel wire)
        {
            if (ConnectedWires.Contains(wire))
            {
                return;
            }

            ConnectedWires.Add(wire);
            WireConnected?.Invoke(wire);
            SemanticsChanged?.Invoke();
        }

        public virtual void EnableWire()
        {
            foreach (var connectedWire in ConnectedWires)
            {
                connectedWire.EnableWire();
            }
        }

        public virtual void DisableWire()
        {
            foreach (var connectedWire in ConnectedWires)
            {
                connectedWire.DisableWire();
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

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
            PropertyChanged += OnTerminalPropertyChanged;
        }

        /// <summary>
        ///     Notifies listeners when the sematics of this terminal have changed.
        /// </summary>
        public virtual event Action SemanticsChanged;

        internal void InitializeType()
        {
            if (_typeName == null)
            {
                return;
            }

            try
            {
                Type = Type.GetType(_typeName) ?? Type.GetType(_typeName, PluginLoader.AssemblyResolver, TypeResolver);
            }
            catch
            {
            }
        }
    }
}