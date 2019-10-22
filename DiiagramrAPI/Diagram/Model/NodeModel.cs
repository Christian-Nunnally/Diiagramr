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
        }

        public NodeModel(string nodeTypeFullName, NodeLibrary dependency)
        {
            Name = nodeTypeFullName;
            Dependency = dependency;
        }

        private NodeModel()
        {
        }

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
        public List<TerminalModel> Terminals { get; set; } = new List<TerminalModel>();

        [DataMember]
        public virtual double Width { get; set; }

        [DataMember]
        public virtual double X { get; set; }

        [DataMember]
        public virtual double Y { get; set; }

        public virtual void AddTerminal(TerminalModel terminal)
        {
            Terminals.Add(terminal);
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
            return !PersistedVariables.ContainsKey(name) ? null : PersistedVariables[name];
        }

        public virtual void InitializePersistedVariableToProperty(PropertyInfo info)
        {
            if (!PersistedVariables.ContainsKey(info.Name))
            {
                SetVariable(info.Name, info.GetValue(NodeViewModel));
            }
        }

        public virtual void RemoveTerminal(TerminalModel terminal)
        {
            terminal.DisconnectWires();
            PropertyChanged -= terminal.NodePropertyChanged;
            Terminals.Remove(terminal);
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
        }
    }
}
