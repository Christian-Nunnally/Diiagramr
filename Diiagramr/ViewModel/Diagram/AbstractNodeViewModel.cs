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
        West
    }

    [AddINotifyPropertyChangedInterface]
    public abstract class AbstractNodeViewModel : Screen
    {
        private readonly List<Action> _dropAndArrangeWhenViewIsLoadedCallbacks = new List<Action>();
        private bool MouseOverBorder { get; set; }
        private bool DroppingTerminal { get; set; }

        public delegate void TerminalConnectedStatusChangedDelegate(TerminalModel terminal);
        public event TerminalConnectedStatusChangedDelegate TerminalConnectedStatusChanged;

        public virtual double X { get; set; }
        public virtual double Y { get; set; }

        public virtual double Width { get; set; }
        public virtual double Height { get; set; }

        public bool TitleVisible => IsSelected || MouseOverBorder;
        public bool IsSelected { get; set; }

        public ICommand DropHandlerCommand { get; set; }

        public IList<TerminalViewModel> TerminalViewModels { get; }
        public IEnumerable<InputTerminalViewModel> InputTerminalViewModels => TerminalViewModels.OfType<InputTerminalViewModel>();
        public IEnumerable<OutputTerminalViewModel> OutputTerminalViewModels => TerminalViewModels.OfType<OutputTerminalViewModel>();

        public virtual string Name { get; set; } = "Node";
        public virtual NodeModel NodeModel { get; set; }

        public AbstractNodeViewModel()
        {
            TerminalViewModels = new ObservableCollection<TerminalViewModel>();
            DropHandlerCommand = new CommandAction(View, View, "DropEventHandler", ActionUnavailableBehaviour.Disable, ActionUnavailableBehaviour.Throw);
        }

        public virtual void InitializeWithNode(NodeModel nodeModel)
        {
            NodeModel = nodeModel;
            NodeModel.NodeViewModel = this;

            LoadTerminalViewModels();
            Initialize();
        }

        public virtual void Uninitialize() { }

        protected virtual void Initialize() { }

        public virtual void SaveNodeVariables() { }

        public virtual void LoadNodeVariables() { }

        private void LoadTerminalViewModels()
        {
            foreach (var terminal in NodeModel.Terminals)
            {
                terminal.PropertyChanged += TerminalOnPropertyChanged;
                TerminalViewModels.Add(TerminalViewModel.CreateTerminalViewModel(terminal));
            }
        }

        public void AddTerminalViewModel(TerminalViewModel terminalViewModel)
        {
            TerminalViewModels.Add(terminalViewModel);
            AddTerminal(terminalViewModel.Terminal);
            DropAndArrangeTerminal(terminalViewModel, terminalViewModel.DefaultDirection);
        }

        private void AddTerminal(TerminalModel terminal)
        {
            terminal.PropertyChanged += TerminalOnPropertyChanged;
            NodeModel.AddTerminal(terminal);
        }

        private void TerminalOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(TerminalModel.ConnectedWire)))
            {
                TerminalConnectedStatusChanged?.Invoke((TerminalModel)sender);
            }
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            if (NodeModel == null) return;
            if (propertyName.Equals(nameof(X))) NodeModel.X = X;
            if (propertyName.Equals(nameof(Y))) NodeModel.Y = Y;
            if (propertyName.Equals(nameof(Width))) NodeModel.Width = Width;
            if (propertyName.Equals(nameof(Height))) NodeModel.Height = Height;
        }

        public void Wiggle()
        {
            OnPropertyChanged(nameof(X));
            OnPropertyChanged(nameof(Y));
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
            }
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
    }
}