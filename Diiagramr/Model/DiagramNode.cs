using Diiagramr.ViewModel.Diagram;
using PropertyChanged;
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

        private AbstractNodeViewModel _nodeViewModel;

        public AbstractNodeViewModel NodeViewModel
        {
            get => _nodeViewModel;
            set
            {
                _nodeViewModel = value;
                _nodeViewModel.LoadNodeVariables();
            }
        }

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
            Terminals.ForEach(t => PropertyChanged += t.NodePropertyChanged);
        }

        public virtual void SetVariable(string name, object value)
        {
            if (!PersistedVariables.ContainsKey(name)) PersistedVariables.Add(name, value);
            else PersistedVariables[name] = value;
        }

        public virtual object GetVariable(string name)
        {
            if (!PersistedVariables.ContainsKey(name)) return null;
            return PersistedVariables[name];
        }

        /// <summary>
        /// Must be called before the node is serialized and saved to disk.
        /// </summary>
        public virtual void PreSave()
        {
            NodeViewModel.SaveNodeVariables();
        }
    }
}