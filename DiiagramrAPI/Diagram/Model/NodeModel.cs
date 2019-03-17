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

        private double _height;

        private Node _nodeViewModel;

        private double _width;

        private double _x;

        private double _y;

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
        public virtual double Height
        {
            get => _height;

            set
            {
                if (Math.Abs(_height - value) < 0.001)
                {
                    return;
                }

                _height = value;
                PresentationChanged?.Invoke();
            }
        }

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
        public virtual double Width
        {
            get => _width;

            set
            {
                if (Math.Abs(_width - value) < 0.001)
                {
                    return;
                }

                _width = value;
                PresentationChanged?.Invoke();
            }
        }

        [DataMember]
        public virtual double X
        {
            get => _x;

            set
            {
                if (Math.Abs(_x - value) < 0.001)
                {
                    return;
                }

                _x = value;
                PresentationChanged?.Invoke();
            }
        }

        [DataMember]
        public virtual double Y
        {
            get => _y;

            set
            {
                if (Math.Abs(_y - value) < 0.001)
                {
                    return;
                }

                _y = value;
                PresentationChanged?.Invoke();
            }
        }

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
