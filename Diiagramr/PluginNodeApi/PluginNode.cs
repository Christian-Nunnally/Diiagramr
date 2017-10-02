using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using Diiagramr.Model;
using Diiagramr.Service;
using Diiagramr.ViewModel.Diagram;
using Stylet;

namespace Diiagramr.PluginNodeApi
{
    public abstract class PluginNode : Screen
    {
        private IEnumerable<PropertyInfo> PluginNodeSettings => GetType().GetProperties().Where(i => Attribute.IsDefined(i, typeof(PluginNodeSetting)));
        private readonly IDictionary<string, PropertyInfo> _pluginNodeSettingCache = new Dictionary<string, PropertyInfo>();

        private readonly List<Action> _viewLoadedActions = new List<Action>();

        public PluginNode()
        {
            TerminalViewModels = new ObservableCollection<TerminalViewModel>();
        }

        private bool MouseOverBorder { get; set; }

        public virtual double X { get; set; }
        public virtual double Y { get; set; }

        public virtual double Width { get; set; }
        public virtual double Height { get; set; }

        public bool TitleVisible => IsSelected || MouseOverBorder;
        public bool IsSelected { get; set; }

        public virtual IList<TerminalViewModel> TerminalViewModels { get; }
        public IEnumerable<InputTerminalViewModel> InputTerminalViewModels => TerminalViewModels.OfType<InputTerminalViewModel>();
        public IEnumerable<OutputTerminalViewModel> OutputTerminalViewModels => TerminalViewModels.OfType<OutputTerminalViewModel>();

        public virtual string Name { get; set; } = "Node";
        public virtual NodeModel NodeModel { get; set; }

        public event Action<TerminalModel> TerminalConnectedStatusChanged;

        public virtual void InitializeWithNode(NodeModel nodeModel)
        {
            NodeModel = nodeModel;
            NodeModel.NodeViewModel = this;

            LoadTerminalViewModels();

            var nodeSetterUpper = new NodeSetup(this);
            SetupNode(nodeSetterUpper);
        }

        private void LoadTerminalViewModels()
        {
            foreach (var terminal in NodeModel.Terminals)
            {
                terminal.PropertyChanged += TerminalOnPropertyChanged;
                TerminalViewModels.Add(TerminalViewModel.CreateTerminalViewModel(terminal));
            }
        }

        public virtual void AddTerminalViewModel(TerminalViewModel terminalViewModel)
        {
            TerminalViewModels.Add(terminalViewModel);
            AddTerminal(terminalViewModel.TerminalModel);
            DropAndArrangeTerminal(terminalViewModel, terminalViewModel.TerminalModel.Direction);
        }

        public virtual void RemoveTerminalViewModel(TerminalViewModel terminalViewModel)
        {
            TerminalViewModels.Remove(terminalViewModel);
            RemoveTerminal(terminalViewModel.TerminalModel);
            FixOtherTerminalsOnEdge(terminalViewModel.TerminalModel.Direction);
        }

        private void AddTerminal(TerminalModel terminal)
        {
            terminal.PropertyChanged += TerminalOnPropertyChanged;
            NodeModel.AddTerminal(terminal);
        }

        private void RemoveTerminal(TerminalModel terminal)
        {
            terminal.PropertyChanged -= TerminalOnPropertyChanged;
            NodeModel.RemoveTerminal(terminal);
        }

        private void TerminalOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(TerminalModel.ConnectedWire)))
                TerminalConnectedStatusChanged?.Invoke((TerminalModel) sender);
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            if (NodeModel == null) return;
            if (propertyName.Equals(nameof(X))) NodeModel.X = X;
            else if (propertyName.Equals(nameof(Y))) NodeModel.Y = Y;
            else if (propertyName.Equals(nameof(Width))) NodeModel.Width = Width;
            else if (propertyName.Equals(nameof(Height))) NodeModel.Height = Height;

            if (!_pluginNodeSettingCache.ContainsKey(propertyName)) return;
            var changedPropertyInfo = _pluginNodeSettingCache[propertyName];
            var value = changedPropertyInfo.GetValue(this);
            NodeModel?.SetVariable(propertyName, value);
        }

        public void DisconnectAllTerminals()
        {
            TerminalViewModels.ForEach(t => t.DisconnectTerminal());
        }

        public void DropEventHandler(object sender, DragEventArgs e)
        {
            var dropPoint = e.GetPosition(View);
            var terminalViewModel = e.Data.GetData(DataFormats.StringFormat) as TerminalViewModel;
            DropAndArrangeTerminal(terminalViewModel, dropPoint.X, dropPoint.Y);
        }

        private void DropAndArrangeTerminal(TerminalViewModel terminal, double x, double y)
        {
            if (terminal == null) return;
            var dropDirection = CalculateClosestDirection(x, y);
            DropAndArrangeTerminal(terminal, dropDirection);
        }

        private void DropAndArrangeTerminal(TerminalViewModel terminal, Direction edge)
        {
            if (View == null)
            {
                _viewLoadedActions.Add(() => DropAndArrangeTerminal(terminal, edge));
                return;
            }

            terminal.SetTerminalDirection(edge);
            var oldEdge = CalculateClosestDirection(terminal.XRelativeToNode, terminal.YRelativeToNode);
            DropTerminalOnEdge(terminal, edge, 0.50f);
            FixOtherTerminalsOnEdge(oldEdge);
            FixOtherTerminalsOnEdge(edge);
        }

        protected override void OnViewLoaded()
        {
            base.OnViewLoaded();
            _viewLoadedActions.ForEach(action => action.Invoke());
            _viewLoadedActions.Clear();
        }

        private void FixOtherTerminalsOnEdge(Direction edge)
        {
            var otherNodesInDirection = TerminalViewModels.Where(t => t.TerminalModel.Direction == edge).ToArray();
            var inc = 1 / (otherNodesInDirection.Length + 1.0f);
            for (var i = 0; i < otherNodesInDirection.Length; i++)
                DropTerminalOnEdge(otherNodesInDirection[i], edge, inc * (i + 1));
        }

        private void DropTerminalOnEdge(TerminalViewModel terminal, Direction edge, float precentAlongEdge)
        {
            switch (edge)
            {
                case Direction.North:
                    terminal.XRelativeToNode = Width * precentAlongEdge;
                    terminal.YRelativeToNode = 0;
                    break;
                case Direction.East:
                    terminal.XRelativeToNode = Width;
                    terminal.YRelativeToNode = Height * precentAlongEdge;
                    break;
                case Direction.South:
                    terminal.XRelativeToNode = Width * precentAlongEdge;
                    terminal.YRelativeToNode = Height;
                    break;
                case Direction.West:
                    terminal.XRelativeToNode = 0;
                    terminal.YRelativeToNode = Height * precentAlongEdge;
                    break;
            }
        }

        private Direction CalculateClosestDirection(double x, double y)
        {
            var closestEastWest = x < Width - x ? Direction.West : Direction.East;
            var closestNorthSouth = y < Height - y ? Direction.North : Direction.South;
            var closestEastWestDistance = Math.Min(x, Width - x);
            var closestNorthSouthDistance = Math.Min(y, Height - y);
            return closestEastWestDistance < closestNorthSouthDistance ? closestEastWest : closestNorthSouth;
        }

        public void MouseEntered(object sender, MouseEventArgs mouseEventArgs)
        {
            MouseOverBorder = true;
        }

        public void MouseLeft(object sender, MouseEventArgs mouseEventArgs)
        {
            MouseOverBorder = false;
        }

        public void ShowInputTerminalLabelsOfType(Type type)
        {
            InputTerminalViewModels.ForEach(terminal => terminal.ShowLabelIfCompatibleType(type));
        }

        public void ShowOutputTerminalLabelsOfType(Type type)
        {
            OutputTerminalViewModels.ForEach(terminal => terminal.ShowLabelIfCompatibleType(type));
        }

        public void HideAllTerminalLabels()
        {
            TerminalViewModels.ForEach(terminal => terminal.TitleVisible = false);
        }

        public virtual void InitializePluginNodeSettings()
        {
            PluginNodeSettings.ForEach(info => _pluginNodeSettingCache.Add(info.Name, info));
            PluginNodeSettings.ForEach(NodeModel.InitializePersistedVariableToProperty);
            PluginNodeSettings.ForEach(info => info.SetValue(this, NodeModel?.GetVariable(info.Name)));
        }

        public virtual void Uninitialize()
        {
        }

        public virtual void Initialize()
        {
        }

        /// <summary>
        ///     All node customization such as turning on/off features and setting node geometry happens here.
        /// </summary>
        public virtual void SetupNode(NodeSetup setup)
        {
        }
    }
}