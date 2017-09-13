using Diiagramr.Model;
using Diiagramr.Service;
using PropertyChanged;
using Stylet;
using Stylet.Xaml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Diiagramr.ViewModel.Diagram
{
    [Serializable]
    public enum Direction
    {
        North,
        East,
        South,
        West,
        None
    }

    [AddINotifyPropertyChangedInterface]
    public abstract class AbstractNodeViewModel : Screen
    {
        public DelegateMapper DelegateMapper { get; set; }

        public delegate void TerminalConnectedStatusChangedDelegate(TerminalModel terminal);

        private static Direction _direction = Direction.North;

        private readonly List<Action> _dropAndArrangeWhenViewIsLoadedCallbacks = new List<Action>();

        public readonly List<OutputTerminalViewModel> OutputTerminals = new List<OutputTerminalViewModel>();

        public bool TitleVisible => IsSelected || MouseOverBorder;

        protected AbstractNodeViewModel()
        {
            TerminalViewModels = new ObservableCollection<TerminalViewModel>();
            DropHandlerCommand = new CommandAction(View, View, "DropEventHandler", ActionUnavailableBehaviour.Disable, ActionUnavailableBehaviour.Throw);
        }

        #region Virtual Methods

        /// <summary>
        /// Called when a node is placed on a diagram.
        /// </summary>
        public virtual void InitializeWithNode(DiagramNode diagramNode)
        {
            DelegateMapper = new DelegateMapper();
            DiagramNode = diagramNode;
            DiagramNode.NodeViewModel = this;

            LoadTerminalViewModels();
        }

        /// <summary>
        /// Called when a node is removed from a diagram.
        /// </summary>
        public virtual void Uninitialize()
        {
        }

        public virtual void OnNodeSaving()
        {
        }

        #endregion

        /// <summary>
        /// Called when a node is placed on a diagram.
        /// </summary>
        private void LoadTerminalViewModels()
        {
            foreach (var terminal in DiagramNode.Terminals)
            {
                terminal.PropertyChanged += TerminalOnPropertyChanged;
            }

            foreach (var terminal in DiagramNode.Terminals.OfType<InputTerminal>())
            {
                TerminalViewModels.Add(new InputTerminalViewModel(terminal));
            }

            foreach (var terminal in DiagramNode.Terminals.OfType<OutputTerminal>())
            {
                TerminalViewModels.Add(new OutputTerminalViewModel(terminal));
            }
        }

        public IList<TerminalViewModel> TerminalViewModels { get; }

        public ICommand DropHandlerCommand { get; set; }

        public IEnumerable<InputTerminalViewModel> InputTerminalViewModels => TerminalViewModels.OfType<InputTerminalViewModel>();
        public IEnumerable<OutputTerminalViewModel> OutputTerminalViewModels => TerminalViewModels.OfType<OutputTerminalViewModel>();

        public virtual double X { get; set; }
        public virtual double Y { get; set; }

        public virtual double Width { get; set; }
        public virtual double Height { get; set; }

        public virtual double WidthPlus2 => Width + 2;
        public virtual double HeightPlus2 => Height + 2;

        public virtual DiagramNode DiagramNode { get; set; }

        public bool MouseOverBorder { get; set; }

        public bool IsSelected { get; set; }

        public virtual string Name { get; }

        public bool DroppingTerminal { get; set; }

        public event TerminalConnectedStatusChangedDelegate TerminalConnectedStatusChanged;

        public InputTerminalViewModel ConstructNewInputTerminal(string name, Type type, Direction defaultDirection, int terminalIndex)
        {
            var inputTerminal = new InputTerminal(name, type, DiagramNode, terminalIndex);
            AddTerminal(inputTerminal);
            return ConstructInputTerminalViewModel(inputTerminal, defaultDirection);
        }

        private InputTerminalViewModel ConstructInputTerminalViewModel(InputTerminal inputTerminal, Direction defaultDirection)
        {
            var inputTerminalViewModel = new InputTerminalViewModel(inputTerminal);
            inputTerminalViewModel.DefaultDirection = defaultDirection;
            AddTerminalViewModel(inputTerminalViewModel);
            return inputTerminalViewModel;
        }

        public OutputTerminalViewModel ConstructNewOutputTerminal(string name, Type type, Direction defaultDirection)
        {
            var outputTerminal = new OutputTerminal(name, type);
            AddTerminal(outputTerminal);
            return ConstructOutputTerminalViewModel(outputTerminal, defaultDirection);
        }

        private OutputTerminalViewModel ConstructOutputTerminalViewModel(OutputTerminal outputTerminal, Direction defaultDirection)
        {
            var outputTerminalViewModel = new OutputTerminalViewModel(outputTerminal);
            outputTerminalViewModel.DefaultDirection = defaultDirection;
            AddTerminalViewModel(outputTerminalViewModel);
            return outputTerminalViewModel;
        }

        protected void RemoveTerminalViewModel(TerminalViewModel terminalViewModel)
        {
            if (!TerminalViewModels.Contains(terminalViewModel)) return;
            RemoveTerminal(terminalViewModel.Terminal);
            TerminalViewModels.Remove(terminalViewModel);
        }

        private void AddTerminalViewModel(TerminalViewModel terminalViewModel)
        {
            TerminalViewModels.Add(terminalViewModel);
            if (terminalViewModel.DefaultDirection != Direction.None)
            {
                DropAndArrangeTerminal(terminalViewModel, terminalViewModel.DefaultDirection);
            }
            else
            {
                PlaceTerminalUsingRoundRobinRules(terminalViewModel);
            }
        }

        private void PlaceTerminalUsingRoundRobinRules(TerminalViewModel terminalViewModel)
        {
            DropAndArrangeTerminal(terminalViewModel, _direction);
            if (_direction == Direction.North) _direction = Direction.South;
            else if (_direction == Direction.South) _direction = Direction.West;
            else if (_direction == Direction.West) _direction = Direction.East;
            else _direction = Direction.North;
        }

        private void AddTerminal(TerminalModel terminal)
        {
            terminal.PropertyChanged += TerminalOnPropertyChanged;
            DiagramNode.AddTerminal(terminal);
        }

        private void RemoveTerminal(TerminalModel terminal)
        {
            terminal.PropertyChanged -= TerminalOnPropertyChanged;
            DiagramNode.RemoveTerminal(terminal);
        }

        private void TerminalOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            var terminal = sender as TerminalModel;
            if (propertyChangedEventArgs.PropertyName.Equals("ConnectedWire"))
            {
                TerminalConnectedStatusChanged?.Invoke(terminal);
            }
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            if (DiagramNode == null) return;
            if (propertyName.Equals(nameof(X))) DiagramNode.X = X;
            if (propertyName.Equals(nameof(Y))) DiagramNode.Y = Y;
            if (propertyName.Equals(nameof(Width))) DiagramNode.Width = Width;
            if (propertyName.Equals(nameof(Height))) DiagramNode.Height = Height;
        }

        public void Wiggle()
        {
            OnPropertyChanged(nameof(X));
            OnPropertyChanged(nameof(Y));
        }

        public void DisconnectAllTerminals()
        {
            foreach (var inputTerminalViewModel in InputTerminalViewModels)
                inputTerminalViewModel.DisconnectTerminal();
            foreach (var outputTerminalViewModel in OutputTerminalViewModels)
                outputTerminalViewModel.DisconnectTerminal();
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

        public void UpdateAllTerminalPositions()
        {
            TerminalViewModels.ForEach(viewModel => DropAndArrangeTerminal(viewModel, viewModel.Terminal.Direction));
        }

        private void DropAndArrangeTerminal(TerminalViewModel terminal, Direction edge)
        {
            if (terminal == null) return;

            if (View == null)
            {
                _dropAndArrangeWhenViewIsLoadedCallbacks.Add(() => DropAndArrangeTerminal(terminal, edge));
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
            foreach (var dropAndArrangeWhenViewIsLoadedCallback in _dropAndArrangeWhenViewIsLoadedCallbacks)
                dropAndArrangeWhenViewIsLoadedCallback.Invoke();
            _dropAndArrangeWhenViewIsLoadedCallbacks.Clear();
        }

        private void FixOtherTerminalsOnEdge(Direction edge)
        {
            var otherNodesInDirection = GetAllTerminalsInDirection(edge).ToArray();
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
                default:
                    PlaceTerminalUsingRoundRobinRules(terminal);
                    break;
            }
            terminal.Terminal.ConnectedWire?.PretendWireMoved();
        }

        private IEnumerable<TerminalViewModel> GetAllTerminalsInDirection(Direction direction)
        {
            return TerminalViewModels.Where(t => CalculateClosestDirection(t.XRelativeToNode, t.YRelativeToNode) == direction);
        }

        private Direction CalculateClosestDirection(double x, double y)
        {
            var closestEastWest = x < Width - x ? Direction.West : Direction.East;
            var closestNorthSouth = y < Height - y ? Direction.North : Direction.South;
            var closestEastWestDistance = Math.Min(x, Width - x);
            var closestNorthSouthDistance = Math.Min(y, Height - y);

            return closestEastWestDistance < closestNorthSouthDistance ? closestEastWest : closestNorthSouth;
        }

        public static void DragOverEventHandler(object sender, DragEventArgs e)
        {
            var control = (Control)sender;
            var contentPresenter = (ContentPresenter)control.DataContext;
            var nodeViewModel = (AbstractNodeViewModel)contentPresenter.Content;
            nodeViewModel.DroppingTerminal = true;
        }

        public void OnNodeSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateAllTerminalPositions();
        }

        public void MouseEntered(object sender, MouseEventArgs mouseEventArgs)
        {
            MouseOverBorder = true;
        }

        public void MouseLeft(object sender, MouseEventArgs mouseEventArgs)
        {
            MouseOverBorder = false;
        }

        public IDictionary<OutputTerminal, object> InvokeInput(int terminalIndex, object arg)
        {
            return DelegateMapper.Invoke(terminalIndex, arg);
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
    }
}