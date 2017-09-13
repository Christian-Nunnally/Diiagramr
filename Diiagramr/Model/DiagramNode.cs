using Diiagramr.ViewModel.Diagram;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Diiagramr.Model
{
    [DataContract]
    [AddINotifyPropertyChangedInterface]
    public class DiagramNode : ModelBase
    {
        [DataMember]
        public string NodeType { get; set; }

        [DataMember]
        public readonly Dictionary<string, object> PersistedVariables = new Dictionary<string, object>();

        public AbstractNodeViewModel NodeViewModel { get; set; }

        private DiagramNode()
        {
            Terminals = new List<TerminalModel>();
        }

        public DiagramNode(string nodeName)
        {
            NodeType = nodeName;
            Terminals = new List<TerminalModel>();
        }

        [DataMember]
        public double X { get; set; }

        [DataMember]
        public double Y { get; set; }

        [DataMember]
        public double Width { get; set; }

        [DataMember]
        public double Height { get; set; }

        [DataMember]
        public bool Initialized { get; set; }

        [DataMember]
        public List<TerminalModel> Terminals { get; set; }

        public void AddTerminal(TerminalModel terminal)
        {
            Terminals.Add(terminal);
            PropertyChanged += terminal.NodePropertyChanged;
        }

        public virtual void SetTerminalsPropertyChanged()
        {
            foreach (var terminal in Terminals)
            {
                PropertyChanged += terminal.NodePropertyChanged;
            }
        }

        public void SetVariable(string name, object value)
        {
            if (!PersistedVariables.ContainsKey(name)) PersistedVariables.Add(name, value);
            else PersistedVariables[name] = value;
        }

        public object GetVariable(string name)
        {
            if (!PersistedVariables.ContainsKey(name)) return null;
            return PersistedVariables[name];
        }

        public void RemoveTerminal(TerminalModel terminal)
        {
            if (!Terminals.Contains(terminal)) throw new InvalidOperationException("Trying to remove a terminal from a node that does not exist.");
            PropertyChanged -= terminal.NodePropertyChanged;
            terminal.DisconnectWire();
            Terminals.Remove(terminal);
        }
    }
}