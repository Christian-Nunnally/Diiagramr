using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace DiiagramrAPI.Diagram.Model
{
    [DataContract]
    public class NodeModel : ModelBase
    {
        [DataMember]
        public readonly Dictionary<string, object> PersistedVariables = new Dictionary<string, object>();

        private Node _nodeViewModel;

        public NodeModel(string nodeTypeFullName)
        {
            Name = nodeTypeFullName;
            Terminals = new List<TerminalModel>();
        }

        public NodeModel(string nodeTypeFullName, NodeLibrary dependency)
        {
            Name = nodeTypeFullName;
            Dependency = dependency;
            Terminals = new List<TerminalModel>();
        }

        private NodeModel()
        {
            Terminals = new List<TerminalModel>();
        }

        /// <summary>
        ///     Notifies listeners when the appearance of this node have changed.
        /// </summary>
        public virtual event Action PresentationChanged;

        /// <summary>
        ///     Notifies listeners when the sematics of this node have changed.
        /// </summary>
        public virtual event Action SemanticsChanged;

        [DataMember]
        public virtual NodeLibrary Dependency { get; set; }

        [DataMember]
        public virtual double Height { get; set; }

        public virtual Node NodeViewModel
        {
            get => _nodeViewModel;

            set
            {
                _nodeViewModel = value;
                _nodeViewModel.InitializePluginNodeSettings();
                Name = _nodeViewModel.GetType().FullName;
            }
        }

        [DataMember]
        public List<TerminalModel> Terminals { get; set; }

        [DataMember]
        public virtual double Width { get; set; }

        [DataMember]
        public virtual double X { get; set; }

        [DataMember]
        public virtual double Y { get; set; }

        public virtual void AddTerminal(TerminalModel terminal)
        {
            Terminals.Add(terminal);
            SemanticsChanged?.Invoke();
            terminal.SemanticsChanged += TerminalSematicsChanged;
            terminal.AddToNode(this);
        }

        public virtual void DisableTerminals()
        {
            Terminals.ForEach(t => t.DisableWire());
        }

        public virtual void EnableTerminals()
        {
            Terminals.ForEach(t => t.EnableWire());
        }

        public virtual object GetVariable(string name)
        {
            if (!PersistedVariables.ContainsKey(name))
            {
                return null;
            }

            return PersistedVariables[name];
        }

        public virtual void InitializePersistedVariableToProperty(PropertyInfo info)
        {
            if (!PersistedVariables.ContainsKey(info.Name))
            {
                SetVariable(info.Name, info.GetValue(NodeViewModel));
            }
        }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
            Terminals.ForEach(t => t.SemanticsChanged += TerminalSematicsChanged);
        }

        public virtual void RemoveTerminal(TerminalModel terminal)
        {
            terminal.DisconnectWires();
            terminal.SemanticsChanged -= TerminalSematicsChanged;
            PropertyChanged -= terminal.NodePropertyChanged;
            Terminals.Remove(terminal);
            TerminalSematicsChanged();
        }

        public virtual void ResetTerminals()
        {
            Terminals.ForEach(t => t.ResetWire());
        }

        public virtual void SetTerminalsPropertyChanged()
        {
            Terminals.ForEach(t => PropertyChanged += t.NodePropertyChanged);
        }

        public virtual void SetVariable(string name, object value)
        {
            if (!PersistedVariables.ContainsKey(name))
            {
                PersistedVariables.Add(name, value);
            }
            else
            {
                PersistedVariables[name] = value;
            }

            SemanticsChanged?.Invoke();
        }

        protected override void OnModelPropertyChanged(string propertyName = null)
        {
            base.OnModelPropertyChanged(propertyName);
            if (nameof(Width) == propertyName
                    || nameof(Height) == propertyName
                    || nameof(X) == propertyName
                    || nameof(Y) == propertyName)
            {
                PresentationChanged?.Invoke();
            }
        }

        private void TerminalSematicsChanged()
        {
            SemanticsChanged?.Invoke();
        }
    }
}
