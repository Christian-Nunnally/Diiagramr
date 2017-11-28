using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using DiiagramrAPI.PluginNodeApi;

namespace DiiagramrAPI.Model
{
    [DataContract]
    public class NodeModel : ModelBase
    {
        [DataMember] public readonly Dictionary<string, object> PersistedVariables = new Dictionary<string, object>();
        private double _height;

        private PluginNode _nodeViewModel;
        private double _width;
        private double _x;
        private double _y;

        private NodeModel()
        {
            Terminals = new List<TerminalModel>();
        }

        public NodeModel(string nodeTypeFullName)
        {
            NodeFullName = nodeTypeFullName;
            Terminals = new List<TerminalModel>();
        }

        public NodeModel(string nodeTypeFullName, DependencyModel dep)
        {
            NodeFullName = nodeTypeFullName;
            Dependency = dep;
            Terminals = new List<TerminalModel>();
        }

        [DataMember]
        public string NodeFullName { get; set; }

        [DataMember]
        public virtual DependencyModel Dependency { get; set; }

        public virtual PluginNode NodeViewModel
        {
            get => _nodeViewModel;
            set
            {
                _nodeViewModel = value;
                _nodeViewModel.InitializePluginNodeSettings();
                NodeFullName = _nodeViewModel.GetType().FullName;
            }
        }

        [DataMember]
        public virtual double X
        {
            get => _x;
            set
            {
                if (_x != value) OnModelPropertyChanged(nameof(X));
                _x = value;
            }
        }

        [DataMember]
        public virtual double Y
        {
            get => _y;
            set
            {
                if (_y != value) OnModelPropertyChanged(nameof(Y));
                _y = value;
            }
        }

        [DataMember]
        public double Width
        {
            get => _width;
            set
            {
                if (_width != value) OnModelPropertyChanged(nameof(Width));
                _width = value;
            }
        }

        [DataMember]
        public double Height
        {
            get => _height;
            set
            {
                if (_height != value) OnModelPropertyChanged(nameof(Height));
                _height = value;
            }
        }

        [DataMember]
        public List<TerminalModel> Terminals { get; set; }

        protected override void OnModelPropertyChanged(string propertyName = null)
        {
            base.OnModelPropertyChanged(propertyName);
            if (propertyName.Equals(nameof(Width))
                || propertyName.Equals(nameof(Height))
                || propertyName.Equals(nameof(X))
                || propertyName.Equals(nameof(Y)))
                PresentationChanged?.Invoke();
        }

        /// <summary>
        ///     Notifies listeners when the sematics of this node have changed.
        /// </summary>
        public virtual event Action SemanticsChanged;

        /// <summary>
        ///     Notifies listeners when the appearance of this diagram have changed.
        /// </summary>
        public virtual event Action PresentationChanged;

        public virtual void AddTerminal(TerminalModel terminal)
        {
            Terminals.Add(terminal);
            SemanticsChanged?.Invoke();
            terminal.SemanticsChanged += TerminalSematicsChanged;
            terminal.AddToNode(this);
        }

        public virtual void SetTerminalsPropertyChanged()
        {
            Terminals.ForEach(t => PropertyChanged += t.NodePropertyChanged);
        }

        public virtual void EnableTerminals()
        {
            Terminals.ForEach(t => t.EnableWire());
        }

        public virtual void ResetTerminals()
        {
            Terminals.ForEach(t => t.ResetWire());
        }

        public virtual void DisableTerminals()
        {
            Terminals.ForEach(t => t.DisableWire());
        }

        public virtual void SetVariable(string name, object value)
        {
            if (!PersistedVariables.ContainsKey(name)) PersistedVariables.Add(name, value);
            else PersistedVariables[name] = value;
            SemanticsChanged?.Invoke();
        }

        public virtual object GetVariable(string name)
        {
            if (!PersistedVariables.ContainsKey(name)) return null;
            return PersistedVariables[name];
        }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
            Terminals.ForEach(t => t.SemanticsChanged += TerminalSematicsChanged);
        }

        private void TerminalSematicsChanged()
        {
            SemanticsChanged?.Invoke();
        }

        public virtual void RemoveTerminal(TerminalModel terminal)
        {
            terminal.DisconnectWires();
            terminal.SemanticsChanged -= TerminalSematicsChanged;
            PropertyChanged -= terminal.NodePropertyChanged;
            Terminals.Remove(terminal);
            TerminalSematicsChanged();
        }

        public virtual void InitializePersistedVariableToProperty(PropertyInfo info)
        {
            if (!PersistedVariables.ContainsKey(info.Name))
                SetVariable(info.Name, info.GetValue(NodeViewModel));
        }
    }
}