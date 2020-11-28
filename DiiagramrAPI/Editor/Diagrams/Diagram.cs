using DiiagramrAPI.Application;
using DiiagramrAPI.Editor.Interactors;
using DiiagramrAPI.Service.Editor;
using DiiagramrCore;
using DiiagramrModel;
using Stylet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace DiiagramrAPI.Editor.Diagrams
{
    /// <summary>
    /// A viewmodel that allows a user to edit a <see cref="Diagram"/>.
    /// </summary>
    public class Diagram : ViewModel<DiagramModel>, IMouseEnterLeaveReaction, IDisposable
    {
        public const double DiagramBorderThickness = 2.0;
        public const double GridSnapInterval = 30.0;
        public const double GridSnapIntervalOffSetCorrection = 13.0;
        public const double NodeBorderWidth = 15.0;
        public const double NodeBorderWidthMinus1 = NodeBorderWidth - 1;
        public const double NodeSelectorBottomMargin = 250;
        public const double NodeSelectorRightMargin = 400;
        public const int DiagramMargin = 100;
        private static readonly Rect BoundingBoxDefault = new Rect(-400, -275, 800, 550);

        /// <summary>
        /// Creates a new instance of <see cref="Diagram"/>.
        /// </summary>
        /// <param name="diagram">The model.</param>
        /// <param name="nodeProvider">A node provider to create nodes.</param>
        /// <param name="diagramInteractors">A list of interactors to enable on this diagram.</param>
        public Diagram(
            DiagramModel diagram,
            INodeProvider nodeProvider,
            IEnumerable<DiagramInteractor> diagramInteractors)
        {
            DiagramInteractionManager = new DiagramInteractionManager(() => diagramInteractors);
            ExecuteWhenViewLoaded(() => Keyboard.Focus(View));
            DiagramModel = diagram;
            DiagramModel.PropertyChanged += DiagramOnPropertyChanged;
            if (diagram.Nodes != null)
            {
                foreach (var nodeModel in diagram.Nodes)
                {
                    var viewModel = nodeProvider.CreateNodeFromModel(nodeModel);
                    AddNodeViewModel(viewModel);
                }
            }
        }

        /// <summary>
        /// The thickness of the extension of the node border beyond the actual size of the node.
        /// </summary>
        public static Thickness NodeBorderExtenderThickness { get; } = new Thickness(NodeBorderWidth - 1);

        /// <summary>
        /// The thickness of the border around the node.
        /// </summary>
        public static Thickness NodeBorderThickness { get; } = new Thickness(NodeBorderWidth);

        /// <summary>
        /// The thickness of the blue selection border around the node.
        /// </summary>
        public static Thickness NodeSelectionBorderThickness { get; } = new Thickness(NodeBorderWidth - 16);

        /// <summary>
        /// The bounding box of the diagram so that the user can keep thier bearings.
        /// </summary>
        public Rect BoundingBox { get; set; }

        /// <summary>
        /// Whether or not to show the bounding box.
        /// </summary>
        public bool ShowBoundingBox { get; set; }

        /// <summary>
        /// Gets or sets the interation manager strategy to use for this diagram.
        /// </summary>
        public DiagramInteractionManager DiagramInteractionManager { get; set; }

        /// <summary>
        /// The model.
        /// </summary>
        public DiagramModel DiagramModel { get; }

        /// <summary>
        /// Gets the diagrams name.
        /// </summary>
        public string Name => DiagramModel.Name;

        /// <summary>
        /// Gets the list of visible nodes on the diagram.
        /// </summary>
        public BindableCollection<Node> Nodes { get; } = new BindableCollection<Node>();

        /// <summary>
        /// Gets or sets how much to pan the diagram in the X direction.
        /// </summary>
        public double PanX { get; set; }

        /// <summary>
        /// Gets or sets how much to pan the diagram in the Y direction.
        /// </summary>
        public double PanY { get; set; }

        /// <summary>
        /// Gets or sets whether to show the dotted snap grid in the background.
        /// </summary>
        public bool ShowSnapGrid { get; set; }

        /// <summary>
        /// Gets or sets the height of the diagram view.
        /// </summary>
        public double ViewHeight { get; set; }

        /// <summary>
        /// Gets or sets the width of the diagram view.
        /// </summary>
        public double ViewWidth { get; set; }

        /// <summary>
        /// Gets or sets the wires visible on the diagram.
        /// </summary>
        public BindableCollection<Wire> Wires { get; } = new BindableCollection<Wire>();

        /// <summary>
        /// Gets or sets the amount of zoom to apply to the diagram.
        /// </summary>
        public double Zoom { get; set; } = 1;

        /// <summary>
        /// Adds a node to the diagram.
        /// </summary>
        /// <param name="node">The node to add.</param>
        public void AddNode(Node node)
        {
            if (node.Model == null)
            {
                throw new InvalidOperationException("Can not add a node to the diagram before it has been initialized");
            }

            DiagramModel.AddNode(node.Model);
            AddNodeViewModel(node);
        }

        /// <summary>
        /// Adds a node to the diagram interactively, allowing the user to move the mouse and click to choose the final position for the node.
        /// </summary>
        /// <param name="node">The node to add.</param>
        public void AddNodeInteractively(Node node)
        {
            AddNode(node);

            var interaction = new DiagramInteractionEventArguments(InteractionType.NodeInserted);
            DiagramInteractionManager.HandleDiagramInput(interaction, this);
        }

        /// <summary>
        /// Adds a wire to the diagram.
        /// </summary>
        /// <param name="wireModel">The wire to add.</param>
        public void AddWire(WireModel wireModel)
        {
            AddWire(new Wire(wireModel, new WirePathingAlgorithum()));
        }

        /// <summary>
        /// Adds a wire to the diagram.
        /// </summary>
        /// <param name="wire">The wire to add.</param>
        public void AddWire(Wire wire)
        {
            if (!Wires.Any(w => w == wire || w.WireModel == wire.WireModel))
            {
                Wires.Add(wire);
            }
        }

        /// <summary>
        /// Calculates the diagram coordinates under a point on the screen.
        /// </summary>
        /// <param name="viewPoint">A point on the screen.</param>
        /// <returns>The diagram coordinates under the given screen point.</returns>
        public Point GetDiagramPointFromViewPoint(Point viewPoint)
        {
            var diagramPointX = GetDiagramPointFromViewPointX(viewPoint.X);
            var diagramPointY = GetDiagramPointFromViewPointY(viewPoint.Y);
            return new Point(diagramPointX, diagramPointY);
        }

        /// <summary>
        /// Calculates the x diagram coordinate under a column on the screen.
        /// </summary>
        /// <param name="x">An x coordinate on the screen.</param>
        /// <returns>The x diagram coordinates under the given screen x coordinate.</returns>
        public double GetDiagramPointFromViewPointX(double x)
        {
            return Zoom == 0 ? x : (x - PanX) / Zoom;
        }

        /// <summary>
        /// Calculates the y diagram coordinate under a row on the screen.
        /// </summary>
        /// <param name="y">An y coordinate on the screen.</param>
        /// <returns>The y diagram coordinates under the given screen y coordinate.</returns>
        public double GetDiagramPointFromViewPointY(double y)
        {
            return Zoom == 0 ? y : (y - PanY) / Zoom;
        }

        /// <summary>
        /// Calculates the x screen coordinate over a column on the diagram.
        /// </summary>
        /// <param name="x">An x coordinate on the diagram.</param>
        /// <returns>The x screen coordinates over the given diagram x coordinate.</returns>
        public double GetViewPointFromDiagramPointX(double x)
        {
            return (Zoom * x) + PanX;
        }

        /// <summary>
        /// Calculates the y screen coordinate over a column on the diagram.
        /// </summary>
        /// <param name="y">An y coordinate on the diagram.</param>
        /// <returns>The y screen coordinates over the given diagram y coordinate.</returns>
        public double GetViewPointFromDiagramPointY(double y)
        {
            return (Zoom * y) + PanY;
        }

        /// <summary>
        /// Removes a node from the diagram.
        /// </summary>
        /// <param name="node">The node to remove.</param>
        public void RemoveNode(Node node)
        {
            DiagramModel.RemoveNode(node.Model);
            Nodes.Remove(node);
            UpdateDiagramBoundingBox();
            ShowBoundingBox = Nodes.Any();
            NotifyOfPropertyChange(nameof(Nodes));
        }

        /// <summary>
        /// Removes a wire from the diagram.
        /// </summary>
        /// <param name="wireModel"></param>
        public void RemoveWire(WireModel wireModel)
        {
            var wire = Wires.FirstOrDefault(w => w.WireModel == wireModel);
            if (wire is object)
            {
                Wires.Remove(wire);
            }
        }

        /// <summary>
        /// Remove a wire from the diagram.
        /// </summary>
        /// <param name="wire"></param>
        public void RemoveWire(Wire wire)
        {
            if (Wires.Contains(wire))
            {
                Wires.Remove(wire);
            }
        }

        /// <summary>
        /// Reset the pan and zoom of the diagram to thier defaul values.
        /// </summary>
        public void ResetPanAndZoom()
        {
            Zoom = 1;
            PanX = ViewWidth / 2;
            PanY = ViewHeight / 2;
        }

        /// <summary>
        /// Returns the value that lies on the diagrams snap grid.
        /// </summary>
        /// <param name="value">The value to snap.</param>
        /// <returns>The snapped value.</returns>
        public double SnapToGrid(double value)
        {
            return CoreUilities.RoundToNearest(value, GridSnapInterval);
        }

        /// <summary>
        /// Highlights all terminals that can be wired to the given terminal.
        /// </summary>
        /// <param name="terminal">The terminal to try wiring to.</param>
        public void HighlightWirableTerminals(TerminalModel terminal)
        {
            var highlightAction = terminal is OutputTerminalModel
                ? (Action<Node>)(node => node.HighlightWirableTerminals<InputTerminal>(terminal.Type))
                : (Action<Node>)(node => node.HighlightWirableTerminals<OutputTerminal>(terminal.Type));
            Nodes.ForEach(highlightAction);
        }

        /// <summary>
        /// Unhighlight all terminals.
        /// </summary>
        public void UnhighlightTerminals()
        {
            Nodes.ForEach(n => n.UnhighlightTerminals());
        }

        /// <summary>
        /// Unselect all nodes.
        /// </summary>
        public void UnselectNodes()
        {
            Nodes.Where(node => node.IsSelected).ForEach(node => node.IsSelected = false);
        }

        /// <summary>
        /// Unselect all terminals.
        /// </summary>
        public void UnselectTerminals()
        {
            Nodes.ForEach(node => node.UnselectTerminals());
        }

        /// <summary>
        /// Updates the bounds of the box in the background of the diagram that surrounds all nodes.
        /// </summary>
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

        /// <summary>
        /// Occurs when the left mouse button is pressed on the diagram.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        public void PreviewLeftMouseButtonDownHandler(object sender, MouseButtonEventArgs e)
        {
            MouseInputHandler(sender, e, InteractionType.LeftMouseDown);
            Keyboard.Focus(View);
        }

        /// <summary>
        /// Occurs when the left mouse button is released on the diagram.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        public void PreviewLeftMouseButtonUpHandler(object sender, MouseButtonEventArgs e)
        {
            MouseInputHandler(sender, e, InteractionType.LeftMouseUp);
        }

        /// <summary>
        /// Occurs when the mouse is moved over the diagram.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        public void PreviewMouseMoveHandler(object sender, MouseEventArgs e)
        {
            MouseInputHandler(sender, e, InteractionType.MouseMoved);
        }

        /// <summary>
        /// Occurs when the mouse wheel is moved on the diagram.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        public void PreviewMouseWheelHandler(object sender, MouseWheelEventArgs e)
        {
            MouseInputHandler(sender, e, InteractionType.MouseWheel);
        }

        /// <summary>
        /// Occurs when the right mouse button is pressed down on the diagram.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        public void PreviewRightMouseButtonDownHandler(object sender, MouseButtonEventArgs e)
        {
            MouseInputHandler(sender, e, InteractionType.RightMouseDown);
        }

        /// <summary>
        /// Occurs when the right mouse button is released down on the diagram.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        public void PreviewRightMouseButtonUpHandler(object sender, MouseButtonEventArgs e)
        {
            MouseInputHandler(sender, e, InteractionType.RightMouseUp);
        }

        /// <summary>
        /// Occurs when a key is pressed on the diagram.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        public void KeyDownHandler(object sender, KeyEventArgs e)
        {
            KeyInputHandler(sender as IInputElement, e, InteractionType.KeyDown);
        }

        /// <summary>
        /// Occurs when a key is released on the diagram.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        public void KeyUpHandler(object sender, KeyEventArgs e)
        {
            KeyInputHandler(sender as IInputElement, e, InteractionType.KeyUp);
        }

        /// <summary>
        /// Occurs when the size of the diagram view changes.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        public void ViewSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ViewWidth = e.NewSize.Width;
            ViewHeight = e.NewSize.Height;
            if (!ShowBoundingBox)
            {
                ResetPanAndZoom();
            }
        }

        /// <inheritdoc/>
        public void MouseEntered()
        {
            // TODO: Remove this and stop implementing the interface.
        }

        /// <inheritdoc/>
        public void MouseLeft()
        {
            // TODO: Remove this and stop implementing the interface.
        }

        public void Dispose()
        {
            Nodes.ForEach(n => n.Dispose());
        }

        private void AddNodeViewModel(Node viewModel)
        {
            Nodes.Add(viewModel);
            ShowBoundingBox = Nodes.Any();
            AddWiresForNode(viewModel);
            UpdateDiagramBoundingBox();
            NotifyOfPropertyChange(nameof(Nodes));
        }

        private void AddWiresForNode(Node viewModel)
        {
            foreach (var inputTerminal in viewModel.Model.Terminals.OfType<InputTerminalModel>())
            {
                inputTerminal.ConnectedWires
                    .Select(w => new Wire(w, new WirePathingAlgorithum()))
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

        private void KeyInputHandler(IInputElement sender, KeyEventArgs e, InteractionType type)
        {
            var mousePosition = Mouse.GetPosition(sender);
            var interaction = new DiagramInteractionEventArguments(type)
            {
                MousePosition = mousePosition,
                Key = e.Key
            };
            DiagramInteractionManager.HandleDiagramInput(interaction, this);
        }

        private void MouseInputHandler(object sender, MouseEventArgs e, InteractionType interactionType)
        {
            var relativeMousePosition = e.GetPosition((IInputElement)sender);
            var interaction = new DiagramInteractionEventArguments(interactionType)
            {
                MousePosition = relativeMousePosition,
            };
            if (e is MouseWheelEventArgs mouseWheelEventArguments)
            {
                interaction.MouseWheelDelta = mouseWheelEventArguments.Delta;
            }

            DiagramInteractionManager.HandleDiagramInput(interaction, this);
        }
    }
}