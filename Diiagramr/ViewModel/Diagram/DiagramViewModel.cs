using Diiagramr.Model;
using Diiagramr.Service;
using Diiagramr.View;
using Stylet;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Point = System.Windows.Point;

namespace Diiagramr.ViewModel.Diagram
{
    public class DiagramViewModel : Screen
    {
        private AbstractNodeViewModel _insertingNodeViewModel;

        public DiagramViewModel(EDiagram diagram, IProvideNodes nodeProvider)
        {
            if (diagram == null) throw new ArgumentNullException(nameof(diagram));
            if (nodeProvider == null) throw new ArgumentNullException(nameof(nodeProvider));

            NodeViewModels = new BindableCollection<AbstractNodeViewModel>();
            WireViewModels = new BindableCollection<WireViewModel>();

            Diagram = diagram;
            Diagram.PropertyChanged += DiagramOnPropertyChanged;
            if (diagram.Nodes != null)
            {
                foreach (var abstractNode in diagram.Nodes)
                {
                    var viewModel = nodeProvider.LoadNodeViewModelFromNode(abstractNode);
                    abstractNode.SetTerminalsPropertyChanged();
                    AddNodeViewModel(viewModel);

                    foreach (var terminal in abstractNode.Terminals)
                    {
                        if (terminal.ConnectedWire == null) continue;
                        AddWireViewModel(terminal.ConnectedWire);
                    }
                }
            }
        }

        public BindableCollection<AbstractNodeViewModel> NodeViewModels { get; set; }

        public BindableCollection<WireViewModel> WireViewModels { get; set; }

        public AbstractNodeViewModel InsertingNodeViewModel
        {
            get { return _insertingNodeViewModel; }
            set
            {
                _insertingNodeViewModel = value;
                if (_insertingNodeViewModel != null)
                {
                    AddNode(_insertingNodeViewModel);
                }
            }
        }

        public EDiagram Diagram { get; }

        public double PanX { get; set; }

        public double PanY { get; set; }

        public double Zoom { get; set; }

        public string Name => Diagram.Name;

        private void DiagramOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName.Equals("Name")) NotifyOfPropertyChange(() => Name);
        }

        private void AddNode(AbstractNodeViewModel viewModel)
        {
            if (viewModel.DiagramNode == null) throw new InvalidOperationException("Can't add a node to the diagram before it's been initialized");
            Diagram.AddNode(viewModel.DiagramNode);
            AddNodeViewModel(viewModel);
        }

        private void AddNodeViewModel(AbstractNodeViewModel viewModel)
        {
            if (!Diagram.Nodes.Contains(viewModel.DiagramNode)) throw new InvalidOperationException("Can't add a view model for a diagramNode that does not exist in the model.");
            viewModel.TerminalConnectedStatusChanged += OnTerminalConnectedStatusChanged;
            NodeViewModels.Add(viewModel);

            foreach (var inputTerminal in viewModel.DiagramNode.Terminals.OfType<InputTerminal>())
            {
                if (inputTerminal.ConnectedWire != null)
                {
                    AddWireViewModel(inputTerminal.ConnectedWire);
                }
            }
        }

        private void ShowTitlesOnTerminalsOfSameType(Terminal terminal)
        {
            foreach (var nodeViewModel in NodeViewModels)
            {
                if (terminal is OutputTerminal)
                {
                    nodeViewModel.ShowInputTerminalLabelsOfType(terminal.Type);
                }
                else
                {
                    nodeViewModel.ShowOutputTerminalLabelsOfType(terminal.Type);
                }
            }
        }

        private void HideAllTerminalLabels()
        {
            foreach (var abstractNodeViewModel in NodeViewModels)
            {
                abstractNodeViewModel.HideAllTerminalLabels();
            }
        }

        private void RemoveNode(AbstractNodeViewModel viewModel)
        {
            Diagram.Nodes.Remove(viewModel.DiagramNode);
            NodeViewModels.Remove(viewModel);
            viewModel.DisconnectAllTerminals();
            viewModel.Uninitialize();
        }

        private void OnTerminalConnectedStatusChanged(Terminal terminal)
        {
            if (terminal.ConnectedWire != null)
            {
                if (WireViewModels.Any(x => x.Wire == terminal.ConnectedWire)) return;
                AddWireViewModel(terminal.ConnectedWire);
            }
            else
            {
                var nullWires = WireViewModels.Where(x => (x.Wire.SourceTerminal == null) || (x.Wire.SinkTerminal == null)).ToArray();
                WireViewModels.RemoveRange(nullWires);
            }
        }

        private void AddWireViewModel(Wire wire)
        {
            if (WireViewModels.Any(x => x.Wire == wire)) return;
            var wireViewModel = new WireViewModel(wire);
            WireViewModels.Add(wireViewModel);
            HideAllTerminalLabels();
        }

        #region Drag Drop Handlers

        public void DragOver(object sender, DragEventArgs e)
        {
            var o = e.Data.GetData(DataFormats.StringFormat);

            var terminal = o as Terminal;
            if (o is Terminal || o is TerminalViewModel)
            {
                HideAllTerminalLabels();
                ShowTitlesOnTerminalsOfSameType(terminal);
                e.Effects = DragDropEffects.Link;
                e.Handled = true;
                return;
            }

            var diagram = o as EDiagram;
            if (diagram != null)
            {
                return;
            }

            e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        public void DragEnter(object sender, DragEventArgs e)
        {
            var o = e.Data.GetData(DataFormats.StringFormat);
            var terminal = o as Terminal;
            if (terminal != null)
            {
                HideAllTerminalLabels();
                ShowTitlesOnTerminalsOfSameType(terminal);
            }

            var diagram = o as EDiagram;
            if (diagram != null)
            {
                return;
            }

            e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        public void DragLeave(object sender, DragEventArgs e)
        {
            var o = e.Data.GetData(DataFormats.StringFormat);
            var diagram = o as EDiagram;
            if (diagram != null)
            {
                return;
            }

            e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        public void DropEventHandler(object sender, DragEventArgs e)
        {
            var nodeViewModel = UnpackNodeViewModelFromSender(sender);
            nodeViewModel.DropEventHandler(sender, e);
        }

        public void DragOverEventHandler(object sender, DragEventArgs e)
        {
            AbstractNodeViewModel.DragOverEventHandler(sender, e);
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

        public void NodeViewLoaded(object sender, RoutedEventArgs e)
        {
            var abstractNodeViewModel = UnpackNodeViewModelFromSender(sender);
            abstractNodeViewModel.Wiggle();
        }

        public void PreviewLeftMouseDownOnBorder(object sender, MouseButtonEventArgs e)
        {
            var abstractNodeViewModel = UnpackNodeViewModelFromSender(sender);
            abstractNodeViewModel.IsSelected = true;
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
            InsertingNodeViewModel = null;
        }

        public void LeftMouseButtonDownHandler(object sender, MouseButtonEventArgs e)
        {
            var relativeMousePosition = GetMousePositionRelativeToSender(sender, e);
            LeftMouseButtonDown(relativeMousePosition);
        }

        public void LeftMouseButtonDown(Point p)
        {
            NodeViewModels.Where(node => node.IsSelected).ForEach(node => node.IsSelected = false);
        }

        public void PreviewRightMouseButtonDownHandler(object sender, MouseButtonEventArgs e)
        {
            Point relativeMousePosition = GetMousePositionRelativeToSender(sender, e);
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
            var inputElement = (IInputElement)sender;
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
            var inputElement = (IInputElement)sender;
            return e.GetPosition(inputElement);
        }

        private static AbstractNodeViewModel UnpackNodeViewModelFromSender(object sender)
        {
            return UnpackNodeViewModelFromControl((Control)sender);
        }

        private static AbstractNodeViewModel UnpackNodeViewModelFromControl(Control control)
        {
            var contentPresenter = control.DataContext as ContentPresenter;
            return (AbstractNodeViewModel)(contentPresenter?.Content ?? control.DataContext);
        }

        #endregion
    }
}