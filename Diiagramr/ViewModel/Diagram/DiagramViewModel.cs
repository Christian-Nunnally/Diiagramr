using Stylet;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Diiagramr.Model;

namespace Diiagramr.ViewModel.Diagram
{
    public class DiagramViewModel : Screen
    {
        private AbstractNodeViewModel _selectedNodeViewModel;

        public DiagramViewModel(EDiagram diagram, NodeSelectorViewModel nodeSelectorViewModel)
        {
            NodeViewModels = new BindableCollection<AbstractNodeViewModel>();
            WireViewModels = new BindableCollection<WireViewModel>();

            Diagram = diagram;
            Diagram.PropertyChanged += DiagramOnPropertyChanged;

            foreach (var abstractNode in diagram.Nodes)
            {
                var viewModel = nodeSelectorViewModel.ConstructAbstractNodeViewModel(abstractNode);
                abstractNode.SetTerminalsPropertyChanged();
                AddNodeViewModel(viewModel);

                foreach (var terminal in abstractNode.Terminals)
                {
                    if (terminal.ConnectedWire == null) continue;
                    AddWireViewModel(terminal.ConnectedWire);
                }
            }
        }

        public BindableCollection<AbstractNodeViewModel> NodeViewModels { get; set; }

        public BindableCollection<WireViewModel> WireViewModels { get; set; }

        private AbstractNodeViewModel SelectedNodeViewModel
        {
            get { return _selectedNodeViewModel; }
            set
            {
                if (value == _selectedNodeViewModel) return;
                if (_selectedNodeViewModel != null) _selectedNodeViewModel.IsSelected = false;
                _selectedNodeViewModel = value;
                if (_selectedNodeViewModel != null) _selectedNodeViewModel.IsSelected = true;
            }

        }

        public AbstractNodeViewModel InsertingNodeViewModel { get; set; }

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

        private void ShowTitlesOnSameTypeTerminals(Terminal terminal)
        {
            HideAllTerminalLabels();
            if (terminal is OutputTerminal)
            {
                foreach (var abstractNodeViewModel in NodeViewModels)
                {
                    foreach (var inputTerminalViewModel in abstractNodeViewModel.InputTerminalViewModels)
                    {
                        if (inputTerminalViewModel.Terminal.Type.IsAssignableFrom(terminal.Type))
                        {
                            inputTerminalViewModel.TitleVisible = true;
                        }
                    }
                }
            }
            else if (terminal is InputTerminal)
            {
                foreach (var abstractNodeViewModel in NodeViewModels)
                {
                    foreach (var inputTerminalViewModel in abstractNodeViewModel.OutputTerminalViewModels)
                    {
                        if (inputTerminalViewModel.Terminal.Type.IsAssignableFrom(terminal.Type))
                        {
                            inputTerminalViewModel.TitleVisible = true;
                        }
                    }
                }
            }
        }

        private void HideAllTerminalLabels()
        {
            foreach (var abstractNodeViewModel in NodeViewModels)
            {
                foreach (var inputTerminalViewModel in abstractNodeViewModel.TerminalViewModels)
                {
                    inputTerminalViewModel.TitleVisible = false;
                }
            }
        }

        private void AddNodeWithRespectToPanAndZoom(AbstractNodeViewModel nodeViewModel)
        {
            nodeViewModel.X = (nodeViewModel.X - PanX) / Zoom;
            nodeViewModel.Y = (nodeViewModel.Y - PanY) / Zoom;
            AddNode(nodeViewModel);
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

        public void LeftMouseButtonDownHandler(object sender, MouseButtonEventArgs e)
        {
            var inputElement = (IInputElement)sender;
            try
            {
                LeftMouseButtonDown(e.GetPosition(inputElement));
            }
            catch (Exception)
            {

            }
        }

        public void DragOver(object sender, DragEventArgs e)
        {
            var o = e.Data.GetData(DataFormats.StringFormat);

            var terminal = o as Terminal;
            if (o is Terminal || o is TerminalViewModel)
            {
                ShowTitlesOnSameTypeTerminals(terminal);
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
                ShowTitlesOnSameTypeTerminals(terminal);
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

        public void LeftMouseButtonDown(Point p)
        {
            SelectedNodeViewModel = null;
            if (InsertingNodeViewModel == null) return;
            AddNodeWithRespectToPanAndZoom(InsertingNodeViewModel);
            InsertingNodeViewModel = null;
        }

        public void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            var inputElement = (IInputElement)sender;
            var mouseLocation = e.GetPosition(inputElement);
            if (InsertingNodeViewModel == null) return;
            InsertingNodeViewModel.X = mouseLocation.X;
            InsertingNodeViewModel.Y = mouseLocation.Y;
        }

        public void NodeViewLoaded(object sender, RoutedEventArgs e)
        {
            var abstractNodeViewModel = UnpackNodeViewModelFromSender(sender);
            abstractNodeViewModel.Wiggle();
        }

        public void PreviewLeftMouseDownOnBorder(object sender, MouseButtonEventArgs e)
        {
            var abstractNodeViewModel = UnpackNodeViewModelFromSender(sender);
            SelectedNodeViewModel = abstractNodeViewModel;
        }

        public void RemoveNodePressed(object sender)
        {
            if (SelectedNodeViewModel != null)
                RemoveNode(SelectedNodeViewModel);
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

        private static AbstractNodeViewModel UnpackNodeViewModelFromSender(object sender)
        {
            return UnpackNodeViewModelFromControl((Control)sender);
        }

        private static AbstractNodeViewModel UnpackNodeViewModelFromControl(Control control)
        {
            var contentPresenter = control.DataContext as ContentPresenter;
            return (AbstractNodeViewModel)(contentPresenter?.Content ?? control.DataContext);
        }

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

        /// <summary>
        /// Called right before the project is saved.
        /// </summary>
        public void SavingProject()
        {
            foreach (var pluginNode in NodeViewModels.OfType<PluginNodeViewModel>())
            {
                pluginNode.NodeSaved();
            }
        }
    }
}