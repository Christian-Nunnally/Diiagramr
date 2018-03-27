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
        [DataMember]
        public readonly Dictionary<string, object> PersistedVariables = new Dictionary<string, object>();

        private NodeModel()
        {
            Terminals = new List<TerminalModel>();
        }

        public NodeModel(string nodeTypeFullName)
        {
            NodeTypeFullName = nodeTypeFullName;
            Terminals = new List<TerminalModel>();
        }

        public NodeModel(string nodeTypeFullName, NodeLibrary dependency)
        {
            NodeTypeFullName = nodeTypeFullName;
            Dependency = dependency;
            Terminals = new List<TerminalModel>();
        }

        /// <summary>
        ///     Notifies listeners when the sematics of this node have changed.
        /// </summary>
        public virtual event Action SemanticsChanged;

        /// <summary>
        ///     Notifies listeners when the appearance of this node have changed.
        /// </summary>
        public virtual event Action PresentationChanged;

        [DataMember]
        public List<TerminalModel> Terminals { get; set; }

        [DataMember]
        public string NodeTypeFullName { get; set; }

        [DataMember]
        public virtual NodeLibrary Dependency { get; set; }

        private double _x;
        [DataMember]
        public virtual double X
        {
            get => _x;
            set
            {
                if (Math.Abs(_x - value) < 0.001) return;
                _x = value;
                PresentationChanged?.Invoke();
            }
        }

        private double _y;
        [DataMember]
        public virtual double Y
        {
            get => _y;
            set
            {
                if (Math.Abs(_y - value) < 0.001) return;
                _y = value;
                PresentationChanged?.Invoke();
            }
        }

        private double _width;
        [DataMember]
        public virtual double Width
        {
            get => _width;
            set
            {
                if (Math.Abs(_width - value) < 0.001) return;
                _width = value; 
                PresentationChanged?.Invoke();
            }
        }

        private double _height;
        [DataMember]
        public virtual double Height
        {
            get => _height;
            set
            {
                if (Math.Abs(_height - value) < 0.001) return;
                _height = value;
                PresentationChanged?.Invoke();
            }
        }

        private PluginNode _nodeViewModel;
        public virtual PluginNode NodeViewModel
        {
            get => _nodeViewModel;
            set
            {
                _nodeViewModel = value;
                _nodeViewModel.InitializePluginNodeSettings();
                NodeTypeFullName = _nodeViewModel.GetType().FullName;
            }
        }

        protected override void OnModelPropertyChanged(string propertyName = null)
        {
            base.OnModelPropertyChanged(propertyName);
            if (nameof(Width) == propertyName
                    || nameof(Height) == propertyName
                    || nameof(X) == propertyName
                    || nameof(Y) == propertyName)
                PresentationChanged?.Invoke();
        }

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