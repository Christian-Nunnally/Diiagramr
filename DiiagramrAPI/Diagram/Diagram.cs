using DiiagramrAPI.Diagram.Interactors;
using DiiagramrAPI.Diagram.Model;
using DiiagramrAPI.Service;
using DiiagramrAPI.Service.Interfaces;
using Stylet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace DiiagramrAPI.Diagram
{
    public class Diagram : Screen
    {
        public const double DiagramBorderThickness = 2.0;
        public const double GridSnapInterval = 30.0;
        public const double GridSnapIntervalOffSetCorrection = 13.0;
        public const double NodeBorderWidth = 15.0;
        public const double NodeBorderWidthMinus1 = NodeBorderWidth - 1;
        public const double NodeSelectorBottomMargin = 250;
        public const double NodeSelectorRightMargin = 400;
        public const int DiagramMargin = 100;
        public static Thickness NodeBorderThickness = new Thickness(NodeBorderWidth);
        public static Thickness NodeSelectionBorderThickness = new Thickness(NodeBorderWidth - 1);
        private readonly ColorTheme _colorTheme;
        private readonly IProvideNodes _nodeProvider;

        public Diagram(
            DiagramModel diagram, 
            IProvideNodes nodeProvider, 
            ColorTheme colorTheme, 
            NodePalette nodeSelectorViewModel,
            IEnumerable<DiagramInteractor> diagramInteractors)
        {
            Terminal.ColorTheme = colorTheme;
            _colorTheme = colorTheme;
            _nodeProvider = nodeProvider ?? throw new ArgumentNullException(nameof(nodeProvider));

            DiagramInteractionManager = new DiagramInteractionManager(() => diagramInteractors);

            Model = diagram;
            Model.PropertyChanged += DiagramOnPropertyChanged;
            if (diagram.Nodes != null)
            {
                foreach (var nodeModel in diagram.Nodes)
                {
                    var viewModel = nodeProvider.LoadNodeViewModelFromNode(nodeModel);
                    nodeModel.SetTerminalsPropertyChanged();
                    AddNodeViewModel(viewModel);
                }
            }
        }

        public bool AreInstructionsVisible => !NodeViewModels.Any();
        public bool BoundingBoxVisible { get; set; }
        public Rect BoundingBox { get; set; }
        public Rect BoundingBoxDefault { get; } = new Rect(100, 100, 800, 550);
        public DiagramModel Model { get; }
        public DiagramInteractionManager DiagramInteractionManager { get; set; }
        public string Name => Model.Name;
        public bool ShowSnapGrid { get; set; }
        public BindableCollection<Node> NodeViewModels { get; set; } = new BindableCollection<Node>();
        public BindableCollection<Wire> WireViewModels { get; set; } = new BindableCollection<Wire>();
        public double PanX { get; set; }
        public double PanY { get; set; }
        public double Zoom { get; set; } = 1;
        public double ViewWidth { get; set; }
        public double ViewHeight { get; set; }

        public void KeyDownHandler(object sender, KeyEventArgs e)
        {
            KeyInputHandler(e, InteractionType.KeyDown);
        }

        public void KeyUpHandler(object sender, KeyEventArgs e)
        {
            KeyInputHandler(e, InteractionType.KeyUp);
        }

        private void KeyInputHandler(KeyEventArgs e, InteractionType type)
        {
            var interaction = new DiagramInteractionEventArguments(type);
            interaction.Key = e.Key;
            DiagramInteractionManager.DiagramInputHandler(interaction, this);
        }

        public void PreviewRightMouseButtonDownHandler(object sender, MouseButtonEventArgs e)
        {
            MouseInputHandler(sender, e, InteractionType.RightMouseDown);
        }

        public void PreviewRightMouseButtonUpHandler(object sender, MouseButtonEventArgs e)
        {
            MouseInputHandler(sender, e, InteractionType.RightMouseUp);
        }

        public void PreviewLeftMouseButtonDownHandler(object sender, MouseButtonEventArgs e)
        {
            MouseInputHandler(sender, e, InteractionType.LeftMouseDown);
            Keyboard.Focus(View);
        }

        public void PreviewLeftMouseButtonUpHandler(object sender, MouseButtonEventArgs e)
        {
            MouseInputHandler(sender, e, InteractionType.LeftMouseUp);
        }

        public void PreviewMouseWheelHandler(object sender, MouseWheelEventArgs e)
        {
            MouseInputHandler(sender, e, InteractionType.MouseWheel);
        }

        public void PreviewMouseMoveHandler(object sender, MouseEventArgs e)
        {
            MouseInputHandler(sender, e, InteractionType.MouseMoved);
        }

        private void MouseInputHandler(object sender, MouseEventArgs e, InteractionType interactionType)
        {
            var relativeMousePosition = e.GetPosition((IInputElement)sender);
            var interaction = new DiagramInteractionEventArguments(interactionType);
            interaction.MousePosition = relativeMousePosition;
            if (e is MouseWheelEventArgs mouseWheelEventArguments)
            {
                interaction.MouseWheelDelta = mouseWheelEventArguments.Delta;
            }
            DiagramInteractionManager.DiagramInputHandler(interaction, this);
        }

        public void AddNode(Node viewModel)
        {
            if (viewModel.Model == null)
            {
                throw new InvalidOperationException("Can not add a node to the diagram before it has been initialized");
            }
            Model.AddNode(viewModel.Model);
            AddNodeViewModel(viewModel);

            var interaction = new DiagramInteractionEventArguments(InteractionType.NodeInserted);
            DiagramInteractionManager.DiagramInputHandler(interaction, this);
        }

        private void AddNodeViewModel(Node viewModel)
        {
            if (!Model.Nodes.Contains(viewModel.Model))
            {
                throw new InvalidOperationException("Can not add a view model for a node model that does not exist in the model.");
            }
            viewModel.WireConnectedToTerminal += WireAddedToDiagram;
            viewModel.WireDisconnectedFromTerminal += WireRemovedFromDiagram;
            NodeViewModels.Add(viewModel);
            BoundingBoxVisible = NodeViewModels.Any();
            AddWiresForNode(viewModel);
            viewModel.Initialize();
            UpdateDiagramBoundingBox();
            NotifyOfPropertyChange(nameof(AreInstructionsVisible));
        }

        private void AddWiresForNode(Node viewModel)
        {
            foreach (var inputTerminal in viewModel.Model.Terminals.Where(t => t.Kind == TerminalKind.Input))
            {
                inputTerminal.ConnectedWires.ForEach(AddWireViewModel);
            }
        }

        private void AddWireViewModel(WireModel wire)
        {
            if (WireViewModels.Any(x => x.Model == wire))
            {
                return;
            }
            var wireViewModel = new Wire(wire)
            {
                ColorTheme = _colorTheme
            };
            WireViewModels.Add(wireViewModel);
            UnHighlightAllTerminals();
        }

        private void DiagramOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(Diagram.Name)))
            {
                NotifyOfPropertyChange(() => Name);
            }
        }

        public double GetDiagramPointFromViewPointX(double x)
        {
            return Zoom == 0 ? x : (x - PanX) / Zoom;
        }

        public double GetDiagramPointFromViewPointY(double y)
        {
            return Zoom == 0 ? y : (y - PanY) / Zoom;
        }

        public double GetViewPointFromDiagramPointX(double x)
        {
            return (Zoom * x) + PanX;
        }

        public double GetViewPointFromDiagramPointY(double y)
        {
            return (Zoom * y) + PanY;
        }

        public void HighlightTerminalsOfSameType(TerminalModel terminal)
        {
            foreach (var nodeViewModel in NodeViewModels)
            {
                if (terminal.Kind == TerminalKind.Output)
                {
                    nodeViewModel.HighlightInputTerminalsOfType(terminal.Type);
                }
                else
                {
                    nodeViewModel.HighlightOutputTerminalsOfType(terminal.Type);
                }
            }
        }

        public void RemoveNode(Node viewModel)
        {
            Model.RemoveNode(viewModel.Model);
            NodeViewModels.Remove(viewModel);
            viewModel.DisconnectAllTerminals();
            viewModel.Uninitialize();
            UpdateDiagramBoundingBox();
            BoundingBoxVisible = NodeViewModels.Any();
            NotifyOfPropertyChange(nameof(AreInstructionsVisible));
        }

        public void UnHighlightAllTerminals()
        {
            NodeViewModels.ForEach(n => n.UnHighlightAllTerminals());
        }

        public void UnselectNodes()
        {
            NodeViewModels.Where(node => node.IsSelected).ForEach(node => node.IsSelected = false);
        }

        public void UnselectTerminals()
        {
            NodeViewModels.ForEach(node => node.UnselectTerminals());
        }

        public void UpdateDiagramBoundingBox()
        {
            var minX = SnapToGrid(Math.Min(NodeViewModels.Select(n => n.X).DefaultIfEmpty(0).Min() - DiagramMargin, BoundingBoxDefault.Left));
            var minY = SnapToGrid(Math.Min(NodeViewModels.Select(n => n.Y).DefaultIfEmpty(0).Min() - DiagramMargin, BoundingBoxDefault.Top));
            var maxX = SnapToGrid(Math.Max(NodeViewModels.Select(n => n.X + n.Width).DefaultIfEmpty(0).Max() + DiagramMargin + NodeBorderThickness.Right + NodeBorderThickness.Left, BoundingBoxDefault.Right));
            var maxY = SnapToGrid(Math.Max(NodeViewModels.Select(n => n.Y + n.Height).DefaultIfEmpty(0).Max() + DiagramMargin + NodeBorderThickness.Top + NodeBorderThickness.Bottom, BoundingBoxDefault.Bottom));
            BoundingBox = new Rect(minX, minY, maxX - minX, maxY - minY);
        }

        public double SnapToGrid(double value)
        {
            return CoreUilities.RoundToNearest(value, GridSnapInterval);
        }

        private void WireAddedToDiagram(WireModel wireModel)
        {
            if (!WireViewModels.Any(x => x.Model == wireModel))
            {
                AddWireViewModel(wireModel);
            }
        }

        private void WireRemovedFromDiagram(WireModel wireModel)
        {
            var wireToRemove = WireViewModels.FirstOrDefault(wire => wire.Model == wireModel);
            if (wireToRemove != null)
            {
                WireViewModels.Remove(wireToRemove);
            }
        }
    }
}
