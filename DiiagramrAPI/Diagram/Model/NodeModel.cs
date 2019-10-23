using DiiagramrAPI.Diagram.Interoperability;
using System.Collections.Generic;
using System.Linq;
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
            if (Terminals.Contains(terminal))
            {
                throw new ModelValidationException(this, "This terminal has already been added");
            }
            if (terminal.ParentNode is object)
            {
                throw new ModelValidationException(this, "Remove terminal from its current node before adding it to this one");
            }

            terminal.ParentNode = this;
            Terminals.Add(terminal);
        }

        public virtual void RemoveTerminal(TerminalModel terminal)
        {
            if (!Terminals.Contains(terminal))
            {
                throw new ModelValidationException(this, "Add terminal before removing it");
            }
            if (terminal.ConnectedWires.Any())
            {
                throw new ModelValidationException(this, "Disconnect terminal before removing it");
            }

            terminal.ParentNode = null;
            Terminals.Remove(terminal);
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

        public virtual void ResetTerminals()
        {
            Terminals.ForEach(t => t.ResetWire());
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

        public bool IsConnected => Terminals.Any(t => t.ConnectedWires.Any());
    }
}
