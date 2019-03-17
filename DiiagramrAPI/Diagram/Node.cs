using DiiagramrAPI.Diagram;
using DiiagramrAPI.Diagram.Model;
using DiiagramrAPI.Service;
using DiiagramrAPI.Shell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace DiiagramrAPI.Diagram
{
    public abstract class Node : ViewModel
    {
        private readonly IDictionary<string, PropertyInfo> _pluginNodeSettingCache = new Dictionary<string, PropertyInfo>();

        private readonly List<Action> _viewLoadedActions = new List<Action>();

        private bool _isSelected;

        public Node()
        {
            TerminalViewModels = new ObservableCollection<Terminal>();
        }

        public event Action<WireModel> WireConnectedToTerminal;
        public event Action<WireModel> WireDisconnectedFromTerminal;

        public virtual IEnumerable<Terminal> DynamicTerminalViewModels =>
            TerminalViewModels.Where(vm => !string.IsNullOrEmpty(vm.TerminalModel.MethodKey));

        public virtual double Height
        {
            get => Model.Height;

            set
            {
                Model.Height = value;
                if (Height < MinimumHeight - 0.05)
                {
                    Height = MinimumHeight;
                }
                FixAllTerminals();
            }
        }

        public IEnumerable<InputTerminal> InputTerminalViewModels =>
            TerminalViewModels.OfType<InputTerminal>();

        public virtual bool IsSelected
        {
            get => _isSelected;

            set
            {
                if (value)
                {
                    SetAdorner(new NodeNameAdornernment(View, this));
                    TerminalViewModels.ForEach(t => t.IsSelected = false);
                }
                else
                {
                    SetAdorner(null);
                }
                _isSelected = value;
            }
        }

        public virtual double MinimumHeight { get; set; }
        public virtual double MinimumWidth { get; set; }
        public virtual string Name { get; set; } = "Node";
        public virtual NodeModel Model { get; set; }

        public IEnumerable<OutputTerminal> OutputTerminalViewModels =>
            TerminalViewModels.OfType<OutputTerminal>();

        public virtual float Weight { get; set; }
        public virtual bool ResizeEnabled { get; set; }
        public virtual bool ResizerVisible => ResizeEnabled && IsSelected;
        public virtual IList<Terminal> TerminalViewModels { get; }
        public virtual bool TitleVisible => IsSelected;

        public virtual double Width
        {
            get => Model.Width;

            set
            {
                Model.Width = value;
                if (Width < MinimumWidth - 0.05)
                {
                    Width = MinimumWidth;
                }

                FixAllTerminals();
            }
        }

        public virtual double X { get; set; }
        public virtual double Y { get; set; }
        private bool IsInitialized { get; set; }

        private IEnumerable<PropertyInfo> PluginNodeSettings =>
            GetType().GetProperties().Where(i => Attribute.IsDefined(i, typeof(NodeSetting)));

        public virtual void AddTerminalViewModel(Terminal terminalViewModel)
        {
            TerminalViewModels.Add(terminalViewModel);
            AddTerminal(terminalViewModel.TerminalModel);
            DropAndArrangeTerminal(terminalViewModel, terminalViewModel.TerminalModel.Direction);
            terminalViewModel.CalculateUTurnLimitsForTerminal(Width, Height);
        }

        public void DisconnectAllTerminals()
        {
            TerminalViewModels.ForEach(t => t.DisconnectTerminal());
        }

        public void DropEventHandler(object sender, DragEventArgs e)
        {
            var dropPoint = e.GetPosition(View);
            var terminalViewModel = e.Data.GetData(DataFormats.StringFormat) as Terminal;
            DropAndArrangeTerminal(terminalViewModel, dropPoint.X, dropPoint.Y);
        }

        public void HighlightInputTerminalsOfType(Type type)
        {
            InputTerminalViewModels.ForEach(terminal => terminal.ShowHighlightIfCompatibleType(type));
        }

        public void HighlightOutputTerminalsOfType(Type type)
        {
            OutputTerminalViewModels.ForEach(terminal => terminal.ShowHighlightIfCompatibleType(type));
        }

        public virtual void Initialize()
        {
        }

        public virtual void InitializePluginNodeSettings()
        {
            PluginNodeSettings.ForEach(info => _pluginNodeSettingCache.Add(info.Name, info));
            PluginNodeSettings.ForEach(Model.InitializePersistedVariableToProperty);
            PluginNodeSettings.ForEach(info => info.SetValue(this, Model?.GetVariable(info.Name)));
        }

        public virtual void InitializeWithNode(NodeModel nodeModel)
        {
            if (IsInitialized)
            {
                return;
            }

            IsInitialized = true;

            Model = nodeModel;
            Model.NodeViewModel = this;

            LoadTerminalViewModels();

            var nodeSetterUpper = new NodeSetup(this);
            SetupNode(nodeSetterUpper);
            InitializeWidthAndHeight();
        }

        public virtual void RemoveTerminalViewModel(Terminal terminalViewModel)
        {
            TerminalViewModels.Remove(terminalViewModel);
            RemoveTerminal(terminalViewModel.TerminalModel);
            FixOtherTerminalsOnEdge(terminalViewModel.TerminalModel.Direction);
        }

        public void UnHighlightAllTerminals()
        {
            TerminalViewModels.ForEach(t => t.HighlightVisible = false);
        }

        public virtual void Uninitialize()
        {
        }

        public void UnselectTerminals()
        {
            TerminalViewModels.ForEach(terminal =>
            {
                terminal.IsSelected = false;
                terminal.HighlightVisible = false;
            });
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            if (Model == null)
            {
                return;
            }

            if (propertyName.Equals(nameof(X)))
            {
                Model.X = X;
            }
            else if (propertyName.Equals(nameof(Y)))
            {
                Model.Y = Y;
            }
            else if (propertyName.Equals(nameof(Width)))
            {
                if (Width < MinimumWidth)
                {
                    Width = MinimumWidth;
                }

                Model.Width = Width;
                FixAllTerminals();
            }
            else if (propertyName.Equals(nameof(Height)))
            {
                if (Height < MinimumHeight)
                {
                    Height = MinimumHeight;
                }

                Model.Height = Height;
                FixAllTerminals();
            }
            if (!_pluginNodeSettingCache.ContainsKey(propertyName))
            {
                return;
            }
            var changedPropertyInfo = _pluginNodeSettingCache[propertyName];
            var value = changedPropertyInfo.GetValue(this);
            Model?.SetVariable(propertyName, value);
        }

        protected override void OnViewLoaded()
        {
            base.OnViewLoaded();
            _viewLoadedActions.ForEach(action => action.Invoke());
            _viewLoadedActions.Clear();
        }

        /// <summary>
        /// All node customization such as turning on/off features and setting node geometry happens here.
        /// </summary>
        protected virtual void SetupNode(NodeSetup setup)
        {
        }

        private void AddTerminal(TerminalModel terminal)
        {
            terminal.WireConnected += TerminalWireConnected;
            terminal.WireDisconnected += TerminalWireDisconnected;
            Model.AddTerminal(terminal);
        }

        private Direction CalculateClosestDirection(double x, double y)
        {
            var closestEastWest = x < Width - x ? Direction.West : Direction.East;
            var closestNorthSouth = y < Height - y ? Direction.North : Direction.South;
            var closestEastWestDistance = Math.Min(x, Width - x);
            var closestNorthSouthDistance = Math.Min(y, Height - y);
            return closestEastWestDistance < closestNorthSouthDistance ? closestEastWest : closestNorthSouth;
        }

        private void DropAndArrangeTerminal(Terminal terminal, double x, double y)
        {
            if (terminal == null)
            {
                return;
            }

            var dropDirection = CalculateClosestDirection(x, y);
            DropAndArrangeTerminal(terminal, dropDirection);
        }

        private void DropAndArrangeTerminal(Terminal terminal, Direction edge)
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
            SetUTurnLimitForTerminals();
        }

        private void DropTerminalOnEdge(Terminal terminal, Direction edge, double precentAlongEdge)
        {
            const int extraSpace = 7;
            var widerWidth = Width + extraSpace * 2;
            var tallerHeight = Height + extraSpace * 2;
            switch (edge)
            {
                case Direction.North:
                    terminal.XRelativeToNode = widerWidth * precentAlongEdge - extraSpace;
                    terminal.YRelativeToNode = 0;
                    break;

                case Direction.East:
                    terminal.XRelativeToNode = Width;
                    terminal.YRelativeToNode = tallerHeight * precentAlongEdge - extraSpace;
                    break;

                case Direction.South:
                    terminal.XRelativeToNode = widerWidth * precentAlongEdge - extraSpace;
                    terminal.YRelativeToNode = Height;
                    break;

                case Direction.West:
                    terminal.XRelativeToNode = 0;
                    terminal.YRelativeToNode = tallerHeight * precentAlongEdge - extraSpace;
                    break;
            }
        }

        private void FixAllTerminals()
        {
            FixOtherTerminalsOnEdge(Direction.North);
            FixOtherTerminalsOnEdge(Direction.East);
            FixOtherTerminalsOnEdge(Direction.South);
            FixOtherTerminalsOnEdge(Direction.West);

            SetUTurnLimitForTerminals();
        }

        private void FixOtherTerminalsOnEdge(Direction edge)
        {
            var otherTerminalsInDirection = TerminalViewModels
                .Where(t => t.TerminalModel.Direction == edge).ToArray();

            var inc = 1 / (otherTerminalsInDirection.Length + 1.0f);
            for (var i = 0; i < otherTerminalsInDirection.Length; i++)
            {
                DropTerminalOnEdge(otherTerminalsInDirection[i], edge, inc * (i + 1.0f));
                otherTerminalsInDirection[i].EdgeIndex = i;
            }
        }

        private void InitializeWidthAndHeight()
        {
            X = Model.X;
            Y = Model.Y;
            if (Model.Width > 1)
            {
                Width = Model.Width;
            }

            if (Model.Height > 1)
            {
                Height = Model.Height;
            }
        }

        private void LoadTerminalViewModels()
        {
            var nondynamicTerminals = Model.Terminals
                .Where(t => string.IsNullOrEmpty(t.MethodKey)).ToArray();

            foreach (var terminal in nondynamicTerminals)
            {
                terminal.WireConnected += TerminalWireConnected;
                terminal.WireDisconnected += TerminalWireDisconnected;
                TerminalViewModels.Add(Terminal.CreateTerminalViewModel(terminal));
            }
        }

        private void RemoveTerminal(TerminalModel terminal)
        {
            terminal.WireConnected += TerminalWireConnected;
            terminal.WireDisconnected += TerminalWireDisconnected;
            Model.RemoveTerminal(terminal);
        }

        private void SetUTurnLimitForTerminals()
        {
            TerminalViewModels.ForEach(t => t.CalculateUTurnLimitsForTerminal(Width, Height));
        }

        private void TerminalWireConnected(WireModel wireModel)
        {
            WireConnectedToTerminal?.Invoke(wireModel);
        }

        private void TerminalWireDisconnected(WireModel wireModel)
        {
            WireDisconnectedFromTerminal?.Invoke(wireModel);
        }
    }
}
