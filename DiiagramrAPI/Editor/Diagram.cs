using DiiagramrAPI.Editor.Interactors;
using DiiagramrAPI.Service.Interfaces;
using DiiagramrAPI.Shell;
using DiiagramrCore;
using DiiagramrModel;
using Stylet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace DiiagramrAPI.Editor
{
    public class Diagram : ViewModel, IMouseEnterLeaveReaction
    {
        public const double DiagramBorderThickness = 2.0;
        public const int DiagramMargin = 100;
        public const double GridSnapInterval = 30.0;
        public const double GridSnapIntervalOffSetCorrection = 13.0;
        public const double NodeBorderWidth = 15.0;
        public const double NodeBorderWidthMinus1 = NodeBorderWidth - 1;
        public const double NodeSelectorBottomMargin = 250;
        public const double NodeSelectorRightMargin = 400;
        private static readonly Rect BoundingBoxDefault = new Rect(-400, -275, 800, 550);
        private readonly IProvideNodes _nodeProvider;

        public Diagram(
            DiagramModel diagram,
            IProvideNodes nodeProvider,
            IEnumerable<DiagramInteractor> diagramInteractors)
        {
            _nodeProvider = nodeProvider ?? throw new ArgumentNullException(nameof(nodeProvider));

            DiagramInteractionManager = new DiagramInteractionManager(() => diagramInteractors);

            Model = diagram;
            Model.PropertyChanged += DiagramOnPropertyChanged;
            if (diagram.Nodes != null)
            {
                foreach (var nodeModel in diagram.Nodes)
                {
                    var viewModel = nodeProvider.LoadNodeViewModelFromNode(nodeModel);
                    AddNodeViewModel(viewModel);
                }
            }
        }

        public static Thickness NodeBorderExtenderThickness { get; } = new Thickness(NodeBorderWidth - 1);
        public static Thickness NodeBorderThickness { get; } = new Thickness(NodeBorderWidth);
        public static Thickness NodeSelectionBorderThickness { get; } = new Thickness(NodeBorderWidth - 16);
        public bool AreInstructionsVisible => !Nodes.Any();
        public Rect BoundingBox { get; set; }
        public bool BoundingBoxVisible { get; set; }
        public DiagramInteractionManager DiagramInteractionManager { get; set; }
        public DiagramModel Model { get; }
        public string Name => Model.Name;
        public BindableCollection<Node> Nodes { get; } = new BindableCollection<Node>();
        public double PanX { get; set; }
        public double PanY { get; set; }
        public bool ShowSnapGrid { get; set; }
        public double ViewHeight { get; set; }
        public double ViewWidth { get; set; }
        public BindableCollection<Wire> Wires { get; } = new BindableCollection<Wire>();
        public double Zoom { get; set; } = 1;

        public void AddNode(Node node)
        {
            if (node.Model == null)
            {
                throw new InvalidOperationException("Can not add a node to the diagram before it has been initialized");
            }
            Model.AddNode(node.Model);
            AddNodeViewModel(node);
        }

        public void AddNodeInteractively(Node node)
        {
            AddNode(node);

            var interaction = new DiagramInteractionEventArguments(InteractionType.NodeInserted);
            DiagramInteractionManager.HandleDiagramInput(interaction, this);
        }

        public void AddWire(WireModel wireModel)
        {
            AddWire(new Wire(wireModel));
        }

        public void AddWire(Wire wire)
        {
            if (!Wires.Any(w => w == wire || w.Model == wire.Model))
            {
                Wires.Add(wire);
            }
        }

        public Point GetDiagramPointFromViewPoint(Point viewPoint)
        {
            var diagramPointX = GetDiagramPointFromViewPointX(viewPoint.X);
            var diagramPointY = GetDiagramPointFromViewPointY(viewPoint.Y);
            return new Point(diagramPointX, diagramPointY);
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
            foreach (var nodeViewModel in Nodes)
            {
                if (terminal is OutputTerminalModel)
                {
                    nodeViewModel.HighlightInputTerminalsOfType(terminal.Type);
                }
                else
                {
                    nodeViewModel.HighlightOutputTerminalsOfType(terminal.Type);
                }
            }
        }

        public void KeyDownHandler(object sender, KeyEventArgs e)
        {
            KeyInputHandler(e, InteractionType.KeyDown);
        }

        public void KeyUpHandler(object sender, KeyEventArgs e)
        {
            KeyInputHandler(e, InteractionType.KeyUp);
        }

        public void MouseEntered()
        {
        }

        public void MouseLeft()
        {
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

        public void PreviewMouseMoveHandler(object sender, MouseEventArgs e)
        {
            MouseInputHandler(sender, e, InteractionType.MouseMoved);
        }

        public void PreviewMouseWheelHandler(object sender, MouseWheelEventArgs e)
        {
            MouseInputHandler(sender, e, InteractionType.MouseWheel);
        }

        public void PreviewRightMouseButtonDownHandler(object sender, MouseButtonEventArgs e)
        {
            MouseInputHandler(sender, e, InteractionType.RightMouseDown);
        }

        public void PreviewRightMouseButtonUpHandler(object sender, MouseButtonEventArgs e)
        {
            MouseInputHandler(sender, e, InteractionType.RightMouseUp);
        }

        public void RemoveNode(Node viewModel)
        {
            var wiresConnectedToNode = viewModel.Terminals.SelectMany(t => t.Model.ConnectedWires);
            var wiresToRemove = Wires.Where(w => wiresConnectedToNode.Contains(w.Model));
            wiresToRemove.ForEach(RemoveWire);
            Model.RemoveNode(viewModel.Model);
            Nodes.Remove(viewModel);
            viewModel.DisconnectAllTerminals();
            viewModel.Uninitialize();
            UpdateDiagramBoundingBox();
            BoundingBoxVisible = Nodes.Any();
            NotifyOfPropertyChange(nameof(AreInstructionsVisible));
        }

        public void RemoveWire(WireModel wireModel)
        {
            var wire = Wires.FirstOrDefault(w => w.Model == wireModel);
            if (wire is object)
            {
                Wires.Remove(wire);
            }
        }

        public void RemoveWire(Wire wire)
        {
            if (Wires.Contains(wire))
            {
                Wires.Remove(wire);
            }
        }

        public void ResetPanAndZoom()
        {
            Zoom = 1;
            PanX = ViewWidth / 2;
            PanY = ViewHeight / 2;
        }

        public double SnapToGrid(double value)
        {
            return CoreUilities.RoundToNearest(value, GridSnapInterval);
        }

        public void UnhighlightTerminals()
        {
            Nodes.ForEach(n => n.UnhighlightTerminals());
        }

        public void UnselectNodes()
        {
            Nodes.Where(node => node.IsSelected).ForEach(node => node.IsSelected = false);
        }

        public void UnselectTerminals()
        {
            Nodes.ForEach(node => node.UnselectTerminals());
        }

        public void UpdateDiagramBoundingBox()
        {
            var visibleNodes = Nodes.Where(x => x.Visible);
            var leftmostNodeLeft = visibleNodes.Select(n => n.X).DefaultIfEmpty(0).Min();
            var topmostNodeTop = visibleNodes.Select(n => n.Y).DefaultIfEmpty(0).Min();
            var rightmostNodeRight = visibleNodes.Select(n => n.X + n.Width).DefaultIfEmpty(0).Max();
            var bottommostNodeBottom = visibleNodes.Select(n => n.Y + n.Height).DefaultIfEmpty(0).Max();

            var minX = SnapToGrid(Math.Min(leftmostNodeLeft - DiagramMargin, BoundingBoxDefault.Left));
            var minY = SnapToGrid(Math.Min(topmostNodeTop - DiagramMargin, BoundingBoxDefault.Top));
            var maxX = SnapToGrid(Math.Max(rightmostNodeRight + DiagramMargin + NodeBorderThickness.Right + NodeBorderThickness.Left, BoundingBoxDefault.Right));
            var maxY = SnapToGrid(Math.Max(bottommostNodeBottom + DiagramMargin + NodeBorderThickness.Top + NodeBorderThickness.Bottom, BoundingBoxDefault.Bottom));
            BoundingBox = new Rect(minX, minY, maxX - minX, maxY - minY);
        }

        public void ViewSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ViewWidth = e.NewSize.Width;
            ViewHeight = e.NewSize.Height;
            if (!BoundingBoxVisible)
            {
                ResetPanAndZoom();
            }
        }

        private void AddNodeViewModel(Node viewModel)
        {
            Nodes.Add(viewModel);
            BoundingBoxVisible = Nodes.Any();
            AddWiresForNode(viewModel);
            viewModel.Initialize();
            UpdateDiagramBoundingBox();
            NotifyOfPropertyChange(nameof(AreInstructionsVisible));
        }

        private void AddWiresForNode(Node viewModel)
        {
            foreach (var inputTerminal in viewModel.Model.Terminals.OfType<InputTerminalModel>())
            {
                inputTerminal.ConnectedWires
                    .Select(w => new Wire(w))
                    .ForEach(AddWire);
            }
        }

        private void DiagramOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(Name)))
            {
                NotifyOfPropertyChange(() => Name);
            }
        }

        private void KeyInputHandler(KeyEventArgs e, InteractionType type)
        {
            var interaction = new DiagramInteractionEventArguments(type) { Key = e.Key };
            DiagramInteractionManager.HandleDiagramInput(interaction, this);
        }

        private void MouseInputHandler(object sender, MouseEventArgs e, InteractionType interactionType)
        {
            var relativeMousePosition = e.GetPosition((IInputElement)sender);
            var interaction = new DiagramInteractionEventArguments(interactionType)
            {
                MousePosition = relativeMousePosition
            };
            if (e is MouseWheelEventArgs mouseWheelEventArguments)
            {
                interaction.MouseWheelDelta = mouseWheelEventArguments.Delta;
            }
            DiagramInteractionManager.HandleDiagramInput(interaction, this);
        }
    }
}