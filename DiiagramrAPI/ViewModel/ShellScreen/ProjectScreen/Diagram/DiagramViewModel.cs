﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DiiagramrAPI.Model;
using DiiagramrAPI.PluginNodeApi;
using DiiagramrAPI.Service;
using DiiagramrAPI.Service.Interfaces;
using DiiagramrAPI.ViewModel.Diagram.CoreNode;
using Stylet;

namespace DiiagramrAPI.ViewModel.Diagram
{
    public class DiagramViewModel : Screen
    {
        private readonly IProvideNodes _nodeProvider;
        private PluginNode _insertingNodeViewModel;

        public DiagramViewModel(DiagramModel diagram, IProvideNodes nodeProvider)
        {
            if (diagram == null) throw new ArgumentNullException(nameof(diagram));
            _nodeProvider = nodeProvider ?? throw new ArgumentNullException(nameof(nodeProvider));

            DiagramControlViewModel = new DiagramControlViewModel(diagram);
            NodeViewModels = new BindableCollection<PluginNode>();
            WireViewModels = new BindableCollection<WireViewModel>();

            Diagram = diagram;
            Diagram.PropertyChanged += DiagramOnPropertyChanged;
            if (diagram.Nodes != null)
                foreach (var abstractNode in diagram.Nodes)
                {
                    var viewModel = nodeProvider.LoadNodeViewModelFromNode(abstractNode);
                    abstractNode.SetTerminalsPropertyChanged();
                    AddNodeViewModel(viewModel);

                    AddWiresForNode(viewModel);
                }
        }

        public bool IsSnapGridVisible => InsertingNodeViewModel != null || NodeBeingDragged;
        public bool NodeBeingDragged { get; set; }

        public DiagramControlViewModel DiagramControlViewModel { get; }

        public BindableCollection<PluginNode> NodeViewModels { get; set; }

        public BindableCollection<WireViewModel> WireViewModels { get; set; }

        public PluginNode InsertingNodeViewModel
        {
            get => _insertingNodeViewModel;
            set
            {
                _insertingNodeViewModel = value;
                if (_insertingNodeViewModel != null)
                    AddNode(_insertingNodeViewModel);
            }
        }

        public DiagramModel Diagram { get; }

        public double PanX { get; set; }

        public double PanY { get; set; }

        public double Zoom { get; set; }

        public string Name => Diagram.Name;

        public bool IsDraggingDiagramCallNode => DraggingDiagramCallNode != null;
        private DiagramCallNodeViewModel DraggingDiagramCallNode { get; set; }

        public string DropDiagramCallText => $"Drop { DraggingDiagramCallNode?.ReferencingDiagramModel?.Name ?? ""} Call";

        private void DiagramOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(Diagram.Name))) NotifyOfPropertyChange(() => Name);
        }

        private void AddNode(PluginNode viewModel)
        {
            if (viewModel.NodeModel == null) throw new InvalidOperationException("Can't add a node to the diagram before it's been initialized");
            Diagram.AddNode(viewModel.NodeModel);
            AddNodeViewModel(viewModel);
        }

        private void AddNodeViewModel(PluginNode viewModel)
        {
            if (!Diagram.Nodes.Contains(viewModel.NodeModel)) throw new InvalidOperationException("Can't add a view model for a nodeModel that does not exist in the model.");
            viewModel.WireConnectedToTerminal += WireAddedToDiagram;
            viewModel.WireDisconnectedFromTerminal += WireRemovedFromDiagram;
            viewModel.DragStarted += NodeDraggingStarted;
            viewModel.DragStopped += NodeDraggingStopped;
            NodeViewModels.Add(viewModel);
            AddWiresForNode(viewModel);

            viewModel.Initialize();
        }

        private void NodeDraggingStarted()
        {
            NodeBeingDragged = true;
        }

        private void NodeDraggingStopped()
        {
            NodeBeingDragged = false;
        }

        private void AddWiresForNode(PluginNode viewModel)
        {
            foreach (var inputTerminal in viewModel.NodeModel.Terminals.Where(t => t.Kind == TerminalKind.Input))
                inputTerminal.ConnectedWires.ForEach(AddWireViewModel);
        }

        private void WireRemovedFromDiagram(WireModel wireModel)
        {
            // Currently handled by wire.disconnected event.
        }

        private void WireAddedToDiagram(WireModel wireModel)
        {
            if (WireViewModels.Any(x => x.WireModel == wireModel)) return;
            AddWireViewModel(wireModel);
        }

        private void ShowTitlesOnTerminalsOfSameType(TerminalModel terminal)
        {
            foreach (var nodeViewModel in NodeViewModels)
                if (terminal.Kind == TerminalKind.Output)
                    nodeViewModel.ShowInputTerminalLabelsOfType(terminal.Type);
                else
                    nodeViewModel.ShowOutputTerminalLabelsOfType(terminal.Type);
        }

        private void HideAllTerminalLabels()
        {
            NodeViewModels.ForEach(n => n.HideAllTerminalLabels());
        }

        private void RemoveNode(PluginNode viewModel)
        {
            Diagram.RemoveNode(viewModel.NodeModel);
            NodeViewModels.Remove(viewModel);
            viewModel.DisconnectAllTerminals();
            viewModel.Uninitialize();
        }

        private void AddWireViewModel(WireModel wire)
        {
            if (WireViewModels.Any(x => x.WireModel == wire)) return;
            var wireViewModel = new WireViewModel(wire);
            wireViewModel.Disconnected += WireViewModelOnDisconnected;
            WireViewModels.Add(wireViewModel);
            HideAllTerminalLabels();
        }

        private void WireViewModelOnDisconnected()
        {
            var nullWires = WireViewModels.Where(x => x.WireModel.SourceTerminal == null || x.WireModel.SinkTerminal == null).ToArray();
            WireViewModels.RemoveRange(nullWires);
        }

        #region Drag Drop Handlers

        public void DragOver(object sender, DragEventArgs e)
        {
            var o = e.Data.GetData(DataFormats.StringFormat);

            if (o is TerminalModel terminal)
            {
                HideAllTerminalLabels();
                ShowTitlesOnTerminalsOfSameType(terminal);
                e.Effects = DragDropEffects.Link;
                return;
            }

            if (o is TerminalViewModel)
            {
                e.Effects = DragDropEffects.Link;
                e.Handled = true;
                return;
            }

            if (o is DiagramModel)
            {
                e.Effects = DragDropEffects.Copy;
                return;
            }

            e.Effects = DragDropEffects.None;
        }

        public void DragEnter(object sender, DragEventArgs e)
        {
            var o = e.Data.GetData(DataFormats.StringFormat);
            if (o is TerminalModel terminal)
            {
                HideAllTerminalLabels();
                ShowTitlesOnTerminalsOfSameType(terminal);
            }

            if (o is DiagramModel diagram)
            {
                e.Effects = DragDropEffects.Move;
                DiagramDragEnter(diagram);
                return;
            }

            e.Effects = DragDropEffects.None;
        }

        public void DiagramDragEnter(DiagramModel diagram)
        {
            var diagramNode = new DiagramCallNodeViewModel();
            diagramNode.InitializeWithNode(new NodeModel(typeof(DiagramCallNodeViewModel).FullName));
            diagramNode.NodeProvider = _nodeProvider;
            diagramNode.SetReferencingDiagramModelIfNotBroken(diagram);
            DraggingDiagramCallNode = diagramNode;
        }

        public void DragLeave(object sender, DragEventArgs e)
        {
            var o = e.Data.GetData(DataFormats.StringFormat);
            if (!(o is DiagramModel)) return;
            DraggingDiagramCallNode = null;
        }

        public void DropEventHandler(object sender, DragEventArgs e)
        {
            var nodeViewModel = UnpackNodeViewModelFromSender(sender);
            nodeViewModel.DropEventHandler(sender, e);
        }

        public void DroppedDiagramCallNode(object sender, DragEventArgs e)
        {
            HideAllTerminalLabels();
            if (DraggingDiagramCallNode == null) return;
            InsertingNodeViewModel = DraggingDiagramCallNode;
            DraggingDiagramCallNode = null;
        }

        #endregion

        #region View Event Handlers

        public void MouseEntered(object sender, MouseEventArgs e)
        {
            var nodeViewModel = UnpackNodeViewModelFromSender(sender);
            nodeViewModel.MouseEntered(sender, e);
        }

        public void MouseLeft(object sender, MouseEventArgs e)
        {
            var nodeViewModel = UnpackNodeViewModelFromSender(sender);
            nodeViewModel.MouseLeft(sender, e);
        }

        public void PreviewLeftMouseDownOnBorderHandler(object sender, MouseButtonEventArgs e)
        {
            var node = UnpackNodeViewModelFromSender(sender);
            var controlKeyPressed = Keyboard.IsKeyDown(Key.RightCtrl) || Keyboard.IsKeyDown(Key.LeftCtrl);
            PreviewLeftMouseButtonDownOnBorder(node, controlKeyPressed);
        }

        public void PreviewLeftMouseButtonDownOnBorder(PluginNode node, bool controlKeyPressed)
        {
            if (!controlKeyPressed) UnselectNodes();
            node.IsSelected = true;
        }

        public void RemoveNodePressed()
        {
            NodeViewModels.Where(node => node.IsSelected).ForEach(RemoveNode);
        }

        public void PreviewLeftMouseButtonDownHandler(object sender, MouseButtonEventArgs e)
        {
            var relativeMousePosition = GetMousePositionRelativeToSender(sender, e);
            PreviewLeftMouseButtonDown(relativeMousePosition);
        }

        public void PreviewLeftMouseButtonDown(Point p)
        {
            if (InsertingNodeViewModel == null) return;

            if (Keyboard.IsKeyDown(Key.RightCtrl) || Keyboard.IsKeyDown(Key.LeftCtrl)) return;

            InsertingNodeViewModel.X = RoundToNearest((int)InsertingNodeViewModel.X, DiagramConstants.GridSnapInterval) - DiagramConstants.NodeBorderWidth + 1;
            InsertingNodeViewModel.Y = RoundToNearest((int)InsertingNodeViewModel.Y, DiagramConstants.GridSnapInterval) - DiagramConstants.NodeBorderWidth + 1;

            InsertingNodeViewModel = null;
        }

        private static int RoundToNearest(int value, int multiple)
        {
            var rem = value % multiple;
            var result = value - rem;
                if (rem > (multiple / 2))
            result += multiple;
            return result;
        }

        public void LeftMouseButtonDownHandler(object sender, MouseButtonEventArgs e)
        {
            var relativeMousePosition = GetMousePositionRelativeToSender(sender, e);
            LeftMouseButtonDown(relativeMousePosition);
        }

        public void LeftMouseButtonDown(Point p)
        {
            UnselectNodes();
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

        public void PreviewRightMouseButtonDown(Point p)
        {
            if (InsertingNodeViewModel == null) return;
            RemoveNode(InsertingNodeViewModel);
            InsertingNodeViewModel = null;
        }

        public void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            var inputElement = (IInputElement) sender;
            var relativeMousePosition = e.GetPosition(inputElement);
            MouseMoved(relativeMousePosition);
        }

        public void MouseMoved(Point mouseLocation)
        {
            if (InsertingNodeViewModel == null) return;
            InsertingNodeViewModel.X = GetPointRelativeToPanAndZoomX(mouseLocation.X) - InsertingNodeViewModel.Width / 2.0 - DiagramConstants.NodeBorderWidth;
            InsertingNodeViewModel.Y = GetPointRelativeToPanAndZoomY(mouseLocation.Y) - InsertingNodeViewModel.Height / 2.0 - DiagramConstants.NodeBorderWidth;
        }

        #endregion

        #region Helper Methods

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
            var inputElement = (IInputElement) sender;
            return e.GetPosition(inputElement);
        }

        private static PluginNode UnpackNodeViewModelFromSender(object sender)
        {
            return UnpackNodeViewModelFromControl((Control) sender);
        }

        private static PluginNode UnpackNodeViewModelFromControl(Control control)
        {
            var contentPresenter = control.DataContext as ContentPresenter;
            return (PluginNode) (contentPresenter?.Content ?? control.DataContext);
        }

        #endregion

    }
}