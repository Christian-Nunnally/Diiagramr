namespace DiiagramrModel
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// The model object representing a node on the diagram.
    /// </summary>
    [DataContract]
    public class NodeModel : ModelBase
    {
        /// <summary>
        /// The dictionary containing the values of all properties on derrived classes marked with <see cref="NodeSettingAttribute"/>.
        /// This is how nodes remember thier custom state when they are serialized and deserialized.
        /// </summary>
        [DataMember]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Needs to be serializable")]
        public readonly Dictionary<string, object> PersistedVariables = new Dictionary<string, object>();

        /// <summary>
        /// Creates a new instance of <see cref="NodeModel"/>.
        /// </summary>
        /// <param name="nodeTypeFullName">The full type name of the type of this node.</param>
        public NodeModel(string nodeTypeFullName)
        {
            Name = nodeTypeFullName;
        }

        /// <summary>
        /// Creates a new instance of <see cref="NodeModel"/>.
        /// </summary>
        /// <param name="nodeTypeFullName">The full type name of the type of this node.</param>
        /// <param name="dependency">The dependency (plugin) required in order to initialize this node.</param>
        public NodeModel(string nodeTypeFullName, NodeLibrary dependency)
        {
            Name = nodeTypeFullName;
            Dependency = dependency;
        }

        private NodeModel()
        {
        }

        /// <summary>
        /// Gets whether this node is currently connected to any wires.
        /// </summary>
        public bool IsConnected => Terminals.Any(t => t.ConnectedWires.Any());

        /// <summary>
        /// Gets or sets this node dependency. This is a plugin that is required in order to use this node.
        /// </summary>
        [DataMember]
        public virtual NodeLibrary Dependency { get; set; }

        /// <summary>
        /// Gets or sets the height of this node on the diagram.
        /// </summary>
        [DataMember]
        public virtual double Height { get; set; }

        /// <summary>
        /// Gets or sets the width of this node on the diagram.
        /// </summary>
        [DataMember]
        public virtual double Width { get; set; }

        /// <summary>
        /// Gets or sets the terminals that are on this node.
        /// </summary>
        [DataMember]
        public ObservableCollection<TerminalModel> Terminals { get; set; } = new ObservableCollection<TerminalModel>();

        /// <summary>
        /// Gets or sets the x position of this node on the diagram.
        /// </summary>
        [DataMember]
        public virtual double X { get; set; }

        /// <summary>
        /// Gets or sets the y position of this node on the diagram.
        /// </summary>
        [DataMember]
        public virtual double Y { get; set; }

        /// <summary>
        /// Adds a terminal to this node.
        /// </summary>
        /// <param name="terminal">The terminal to add.</param>
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

        /// <summary>
        /// Removes a terminal from this node.
        /// </summary>
        /// <param name="terminal">The terminal to remove.</param>
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

        /// <summary>
        /// Gets a persisted variable value by its key.
        /// </summary>
        /// <param name="name">The name of the variable to get.</param>
        /// <returns>The value of the persisted variable.</returns>
        public virtual object GetVariable(string name)
        {
            PersistedVariables.TryGetValue(name, out object value);
            return value;
        }

        /// <summary>
        /// Sets a variable to be persisted when this node is saved.
        /// </summary>
        /// <param name="name">The name of the variable to persist.</param>
        /// <param name="value">The value to set the variable to.</param>
        public virtual void SetVariable(string name, object value)
        {
            PersistedVariables[name] = value;
        }

        /// <inheritdoc/>
        public override ModelBase Copy()
        {
            var copy = (NodeModel)base.Copy();
            foreach (var terminal in copy.Terminals.ToArray())
            {
                foreach (var connectedWire in terminal.ConnectedWires.ToArray())
                {
                    terminal.DisconnectWire(connectedWire, connectedWire.SinkTerminal == terminal ? connectedWire.SourceTerminal : connectedWire.SinkTerminal);
                }
            }
            return copy;
        }
    }
}