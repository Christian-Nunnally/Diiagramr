using DiiagramrAPI.Model;
using DiiagramrAPI.PluginNodeApi;
using DiiagramrAPI.Service;
using DiiagramrAPI.Service.Interfaces;
using DiiagramrAPI.ViewModel.Diagram;
using Stylet;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DiiagramrAPI.ViewModel.ProjectScreen.Diagram
{
    public class DiagramViewModel : Screen
    {
        private readonly ColorTheme _colorTheme;
        public const int DiagramMargin = 100;
        public const double NodeBorderWidth = 15.0;
        public const double NodeBorderWidthMinus1 = NodeBorderWidth - 1;
        public const double NodeSelectorRightMargin = 400;
        public const double NodeSelectorBottomMargin = 250;

        public const double GridSnapInterval = 30.0;
        public const double DiagramBorderThickness = 2.0;
        public const double GridSnapIntervalOffSetCorrection = 13.0;
        public static Thickness NodeBorderThickness = new Thickness(NodeBorderWidth);
        public static Thickness NodeSelectionBorderThickness = new Thickness(NodeBorderWidth - 1);

        private readonly IProvideNodes _nodeProvider;

        public DiagramViewModel(DiagramModel diagram, IProvideNodes nodeProvider, ColorTheme colorTheme, NodeSelectorViewModel nodeSelectorViewModel)
        {
            if (colorTheme != null)
            {
                TerminalViewModel.ColorTheme = colorTheme;
            }

            if (diagram == null)
            {
                throw new ArgumentNullException(nameof(diagram));
            }

            _colorTheme = colorTheme;
            _nodeProvider = nodeProvider ?? throw new ArgumentNullException(nameof(nodeProvider));

            if (nodeSelectorViewModel != null)
            {
                NodeSelectorViewModel = nodeSelectorViewModel;
                NodeSelectorViewModel.NodeSelected += node => BeginInsertingNode(node, true);
            }

            DiagramControlViewModel = new DiagramControlViewModel(diagram);
            NodeViewModels = new BindableCollection<PluginNode>();
            WireViewModels = new BindableCollection<WireViewModel>();

            Diagram = diagram;
            Diagram.PropertyChanged += DiagramOnPropertyChanged;
            if (diagram.Nodes != null)
            {
                foreach (var nodeModel in diagram.Nodes)
                {
                    var viewModel = nodeProvider.LoadNodeViewModelFromNode(nodeModel);
                    nodeModel.SetTerminalsPropertyChanged();

                    AddNodeViewModel(viewModel);
                    AddWiresForNode(viewModel);
                }
            }
        }

        public NodeSelectorViewModel NodeSelectorViewModel { get; set; }

        public bool IsSnapGridVisible => InsertingNodeViewModel != null || NodeBeingDragged;
        public bool NodeBeingDragged { get; set; }
        public bool AreInstructionsVisible => !NodeViewModels.Any();
        public Rect BoundingBox { get; set; }
        public Rect DefaultBoundingBox { get; } = new Rect(100, 100, 800, 600);
        public DiagramControlViewModel DiagramControlViewModel { get; }
        public BindableCollection<PluginNode> NodeViewModels { get; set; }
        public BindableCollection<WireViewModel> WireViewModels { get; set; }
        public PluginNode InsertingNodeViewModel { get; set; }
        public DiagramModel Diagram { get; }

        public double PanX { get; set; }
        public double PanY { get; set; }
        public double Zoom { get; set; }

        public string Name => Diagram.Name;

        private void DiagramOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(Diagram.Name)))
            {
                NotifyOfPropertyChange(() => Name);
            }
        }

        private void AddNode(PluginNode viewModel)
        {
            if (viewModel.NodeModel == null)
            {
                throw new InvalidOperationException("Can't add a node to the diagram before it's been initialized");
            }

            Diagram.AddNode(viewModel.NodeModel);
            AddNodeViewModel(viewModel);
            viewModel.TerminalWiringModeChanged += PluginNodeOnWiringModeChanged;
        }

        private void PluginNodeOnWiringModeChanged(TerminalViewModel terminalViewModel, bool enabled)
        {
            UnHighlightAllTerminals();
            UnselectNodes();
            if (enabled)
            {
                HighlightTerminalsOfSameType(terminalViewModel.TerminalModel);
            }
        }

        private void AddNodeViewModel(PluginNode viewModel)
        {
            if (!Diagram.Nodes.Contains(viewModel.NodeModel))
            {
                throw new InvalidOperationException("Can't add a view model for a nodeModel that does not exist in the model.");
            }

            viewModel.WireConnectedToTerminal += WireAddedToDiagram;
            viewModel.WireDisconnectedFromTerminal += WireRemovedFromDiagram;
            viewModel.DragStarted += NodeDraggingStarted;
            viewModel.DragStopped += NodeDraggingStopped;
            NodeViewModels.Add(viewModel);
            AddWiresForNode(viewModel);
            viewModel.Initialize();
            UpdateDiagramBoundingBox();
            NotifyOfPropertyChange(nameof(AreInstructionsVisible));
        }

        private void NodeDraggingStarted()
        {
            NodeBeingDragged = true;
        }

        private void NodeDraggingStopped()
        {
            NodeBeingDragged = false;
            UpdateDiagramBoundingBox();
        }

        private void UpdateDiagramBoundingBox()
        {
            if (NodeViewModels.Count == 0)
            {
                BoundingBox = new Rect(-DiagramMargin, -DiagramMargin, DiagramMargin, DiagramMargin);
                NotifyOfPropertyChange(nameof(BoundingBox));
                return;
            }

            var minX = Math.Min(NodeViewModels.Select(n => n.X).Min() - DiagramMargin, DefaultBoundingBox.Left);
            var minY = Math.Min(NodeViewModels.Select(n => n.Y).Min() - DiagramMargin, DefaultBoundingBox.Top);
            var maxX = Math.Max(NodeViewModels.Select(n => n.X + n.Width).Max() + DiagramMargin + NodeBorderThickness.Right + NodeBorderThickness.Left, DefaultBoundingBox.Right);
            var maxY = Math.Max(NodeViewModels.Select(n => n.Y + n.Height).Max() + DiagramMargin + NodeBorderThickness.Top + NodeBorderThickness.Bottom, DefaultBoundingBox.Bottom);
            var width = maxX - minX;
            var height = maxY - minY;

            var snappedX = CoreUilities.RoundToNearest(minX, GridSnapInterval);
            var snappedY = CoreUilities.RoundToNearest(minY, GridSnapInterval);
            var snappedWidth = CoreUilities.RoundToNearest(width, GridSnapInterval);
            var snappedHeight = CoreUilities.RoundToNearest(height, GridSnapInterval);

            BoundingBox = new Rect(snappedX, snappedY, snappedWidth, snappedHeight);
            NotifyOfPropertyChange(nameof(BoundingBox));
        }

        private void AddWiresForNode(PluginNode viewModel)
        {
            foreach (var inputTerminal in viewModel.NodeModel.Terminals.Where(t => t.Kind == TerminalKind.Input))
            {
                inputTerminal.ConnectedWires.ForEach(AddWireViewModel);
            }
        }

        private void WireRemovedFromDiagram(WireModel wireModel)
        {
            var wireToRemove = WireViewModels.FirstOrDefault(wire => wire.WireModel == wireModel);
            if (wireToRemove != null)
            {
                WireViewModels.Remove(wireToRemove);
            }
        }

        private void WireAddedToDiagram(WireModel wireModel)
        {
            if (WireViewModels.Any(x => x.WireModel == wireModel))
            {
                return;
            }

            AddWireViewModel(wireModel);
        }

        private void HighlightTerminalsOfSameType(TerminalModel terminal)
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

        private void UnHighlightAllTerminals()
        {
            NodeViewModels.ForEach(n => n.UnHighlightAllTerminals());
        }

        private void RemoveNode(PluginNode viewModel)
        {
            Diagram.RemoveNode(viewModel.NodeModel);
            NodeViewModels.Remove(viewModel);
            viewModel.TerminalWiringModeChanged -= PluginNodeOnWiringModeChanged;
            viewModel.DisconnectAllTerminals();
            viewModel.Uninitialize();
            UpdateDiagramBoundingBox();
            NotifyOfPropertyChange(nameof(AreInstructionsVisible));
        }

        private void AddWireViewModel(WireModel wire)
        {
            if (WireViewModels.Any(x => x.WireModel == wire))
            {
                return;
            }

            var wireViewModel = new WireViewModel(wire)
            {
                ColorTheme = _colorTheme
            };
            WireViewModels.Add(wireViewModel);
            UnHighlightAllTerminals();
        }

        private void BeginInsertingNode(PluginNode node, bool insertCopy = false)
        {
            var nodeTypeName = node.GetType().FullName;
            var nodeToInsert = insertCopy ? _nodeProvider.CreateNodeViewModelFromName(nodeTypeName) : node;
            nodeToInsert.X = 400;
            nodeToInsert.Y = 400;
            AddNode(nodeToInsert);
            InsertingNodeViewModel = nodeToInsert;
        }

        private void CancelInsertingNode()
        {
            RemoveNode(InsertingNodeViewModel);
            InsertingNodeViewModel = null;
        }

        public void MouseEntered(object sender, MouseEventArgs e)
        {
            var nodeViewModel = UnpackNodeViewModelFromSender(sender);
            nodeViewModel?.MouseEntered(sender, e);
        }

        public void MouseLeft(object sender, MouseEventArgs e)
        {
            var nodeViewModel = UnpackNodeViewModelFromSender(sender);
            nodeViewModel?.MouseLeft(sender, e);
        }

        public void PreviewLeftMouseDownOnBorderHandler(object sender, MouseButtonEventArgs e)
        {
            var node = UnpackNodeViewModelFromSender(sender);
            if (node == null)
            {
                return;
            }
            var controlKeyPressed = Keyboard.IsKeyDown(Key.RightCtrl) || Keyboard.IsKeyDown(Key.LeftCtrl);
            var altKeyPressed = Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt);
            PreviewLeftMouseButtonDownOnBorder(node, controlKeyPressed, altKeyPressed);
        }

        public void PreviewLeftMouseButtonDownOnBorder(PluginNode node, bool controlKeyPressed, bool altKeyPressed)
        {
            if (altKeyPressed)
            {
                BeginInsertingNode(node, true);
                return;
            }
            if (!controlKeyPressed)
            {
                UnselectNodes();
            }

            node.IsSelected = true;
        }

        public void RemoveSelectedNodes()
        {
            NodeViewModels.Where(node => node.IsSelected).ForEach(RemoveNode);
        }

        public void NodeHelpPressed()
        {
            //todo
        }

        public void PreviewLeftMouseButtonDownHandler(object sender, MouseButtonEventArgs e)
        {
            var relativeMousePosition = GetMousePositionRelativeToSender(sender, e);
            var controlKeyPressed = Keyboard.IsKeyDown(Key.RightCtrl) || Keyboard.IsKeyDown(Key.LeftCtrl);
            if (InsertingNodeViewModel != null)
            {
                e.Handled = true;
            }

            PreviewLeftMouseButtonDown(relativeMousePosition, controlKeyPressed);
        }

        public void PreviewLeftMouseButtonDown(Point p, bool controlKeyPressed = true)
        {
            if (InsertingNodeViewModel == null)
            {
                return;
            }

            if (!controlKeyPressed)
            {
                InsertingNodeViewModel.X = CoreUilities.RoundToNearest((int)InsertingNodeViewModel.X, GridSnapInterval);
                InsertingNodeViewModel.Y = CoreUilities.RoundToNearest((int)InsertingNodeViewModel.Y, GridSnapInterval);
            }

            InsertingNodeViewModel = null;
        }

        public void LeftMouseButtonDownHandler(object sender, MouseButtonEventArgs e)
        {
            var relativeMousePosition = GetMousePositionRelativeToSender(sender, e);
            LeftMouseButtonDown(relativeMousePosition);
        }

        public void LeftMouseButtonDown(Point p)
        {
            UnselectNodes();
            UnselectTerminals();
        }

        private void UnselectTerminals()
        {
            NodeViewModels.ForEach(node => node.UnselectTerminals());
        }

        private void UnselectNodes()
        {
            NodeViewModels.Where(node => node.IsSelected).ForEach(node => node.IsSelected = false);
        }

        public void PreviewRightMouseButtonDownHandler(object sender, MouseButtonEventArgs e)
        {
            var relativeMousePosition = GetMousePositionRelativeToSender(sender, e);
            PreviewRightMouseButtonDown(relativeMousePosition);
        }

        public void PreviewRightMouseButtonDown(Point point)
        {
            if (InsertingNodeViewModel == null)
            {
                ShowNodeSelector(point);
            }
            else
            {
                CancelInsertingNode();
            }
        }

        private void ShowNodeSelector(Point point)
        {
            var availableWidth = View != null ? View.RenderSize.Width : 0;
            var availableHeight = View != null ? View.RenderSize.Height : 0;

            NodeSelectorViewModel.RightPosition = point.X < availableWidth - NodeSelectorRightMargin ? point.X : availableWidth - NodeSelectorRightMargin;
            NodeSelectorViewModel.TopPosition = point.Y < availableHeight - NodeSelectorBottomMargin ? point.Y : availableHeight - NodeSelectorBottomMargin;
            NodeSelectorViewModel.Visible = true;
        }

        public void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            var inputElement = (IInputElement)sender;
            var relativeMousePosition = e.GetPosition(inputElement);
            MouseMoved(relativeMousePosition);
        }

        public void MouseMoved(Point mouseLocation)
        {
            if (InsertingNodeViewModel == null)
            {
                return;
            }

            InsertingNodeViewModel.X = GetPointRelativeToPanAndZoomX(mouseLocation.X) - InsertingNodeViewModel.Width / 2.0 - NodeBorderWidth;
            InsertingNodeViewModel.Y = GetPointRelativeToPanAndZoomY(mouseLocation.Y) - InsertingNodeViewModel.Height / 2.0 - NodeBorderWidth;
        }

        public void KeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                RemoveSelectedNodes();
            }
        }

        private double GetPointRelativeToPanAndZoomX(double x)
        {
            return Zoom == 0 ? x : (x - PanX) / Zoom;
        }

        private double GetPointRelativeToPanAndZoomY(double y)
        {
            return Zoom == 0 ? y : (y - PanY) / Zoom;
        }

        private static Point GetMousePositionRelativeToSender(object sender, MouseButtonEventArgs e)
        {
            var inputElement = (IInputElement)sender;
            return e.GetPosition(inputElement);
        }

        private static PluginNode UnpackNodeViewModelFromSender(object sender)
        {
            return UnpackNodeViewModelFromControl((Control)sender);
        }

        private static PluginNode UnpackNodeViewModelFromControl(Control control)
        {
            var contentPresenter = control.DataContext as ContentPresenter;
            return (contentPresenter?.Content ?? control.DataContext) as PluginNode;
        }
    }
}
