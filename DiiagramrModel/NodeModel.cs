namespace DiiagramrModel
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract]
    public class NodeModel : ModelBase
    {
        [DataMember]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Needs to be serializable")]
        public readonly Dictionary<string, object> PersistedVariables = new Dictionary<string, object>();

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

        public bool IsConnected => Terminals.Any(t => t.ConnectedWires.Any());

        [DataMember]
        public virtual NodeLibrary Dependency { get; set; }

        [DataMember]
        public virtual double Height { get; set; }

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
            PersistedVariables.TryGetValue(name, out object value);
            return value;
        }

        public virtual void SetVariable(string name, object value)
        {
            PersistedVariables[name] = value;
        }
    }
}