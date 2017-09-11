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
            Terminals = new List<Terminal>();
        }

        public DiagramNode(string nodeName)
        {
            NodeType = nodeName;
            Terminals = new List<Terminal>();
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
        public List<Terminal> Terminals { get; set; }

        public void AddTerminal(Terminal terminal)
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

        public int GetIntVariable(string name)
        {
            return (int)GetVariable(name);
        }

        public string GetStringVariable(string name)
        {
            return (string)GetVariable(name);
        }

        public bool GetBoolVariable(string name)
        {
            return (bool)GetVariable(name);
        }

        public void RemoveTerminal(Terminal terminal)
        {
            if (!Terminals.Contains(terminal)) throw new InvalidOperationException("Trying to remove a terminal from a node that does not exist.");
            PropertyChanged -= terminal.NodePropertyChanged;
            terminal.DisconnectWire();
            Terminals.Remove(terminal);
        }
    }
}