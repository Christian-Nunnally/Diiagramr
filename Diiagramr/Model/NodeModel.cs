using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Diiagramr.ViewModel.Diagram;
using PropertyChanged;

namespace Diiagramr.Model
{
    [DataContract]
    [AddINotifyPropertyChangedInterface]
    public class NodeModel : ModelBase
    {
        [DataMember] public readonly Dictionary<string, object> PersistedVariables = new Dictionary<string, object>();

        private AbstractNodeViewModel _nodeViewModel;

        private NodeModel()
        {
            Terminals = new List<TerminalModel>();
        }

        public NodeModel(string nodeTypeFullName)
        {
            NodeFullName = nodeTypeFullName;
            Terminals = new List<TerminalModel>();
        }

        [DataMember]
        public string NodeFullName { get; set; }

        public AbstractNodeViewModel NodeViewModel
        {
            get => _nodeViewModel;
            set
            {
                _nodeViewModel = value;
                _nodeViewModel.SetupPluginNodeSettings();
                _nodeViewModel.LoadNodeVariables();
            }
        }

        [DataMember]
        public virtual double X { get; set; }

        [DataMember]
        public virtual double Y { get; set; }

        [DataMember]
        public double Width { get; set; }

        [DataMember]
        public double Height { get; set; }

        [DataMember]
        public List<TerminalModel> Terminals { get; set; }

        /// <summary>
        ///     Notifies listeners when the sematics of this node have changed.
        /// </summary>
        public virtual event Action SemanticsChanged;

        public void AddTerminal(TerminalModel terminal)
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

        public void RemoveTerminal(TerminalModel terminal)
        {
            terminal.DisconnectWire();
            terminal.SemanticsChanged -= TerminalSematicsChanged;
            PropertyChanged -= terminal.NodePropertyChanged;
            Terminals.Remove(terminal);
            TerminalSematicsChanged();
        }
    }
}