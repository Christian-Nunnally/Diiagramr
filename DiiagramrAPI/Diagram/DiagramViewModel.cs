using DiiagramrAPI.Diagram.Interactors;
using DiiagramrAPI.Diagram.Model;
using DiiagramrAPI.PluginNodeApi;
using DiiagramrAPI.Service;
using DiiagramrAPI.Service.Interfaces;
using Stylet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DiiagramrAPI.Diagram
{
    public class DiagramViewModel : Screen
    {
        public const double DiagramBorderThickness = 2.0;
        public const double GridSnapInterval = 30.0;
        public const double GridSnapIntervalOffSetCorrection = 13.0;
        public const double NodeBorderWidth = 15.0;
        public const double NodeBorderWidthMinus1 = NodeBorderWidth - 1;

        internal void PanNotify()
        {
            NotifyOfPropertyChange(nameof(PanX));
            NotifyOfPropertyChange(nameof(PanY));
        }

        public const double NodeSelectorBottomMargin = 250;
        public const double NodeSelectorRightMargin = 400;
        public const int DiagramMargin = 100;
        public static Thickness NodeBorderThickness = new Thickness(NodeBorderWidth);
        public static Thickness NodeSelectionBorderThickness = new Thickness(NodeBorderWidth - 1);
        private DiagramModel object1;
        private IProvideNodes object2;
        private object p;
        private NodeSelectorViewModel object3;
        private List<DiagramInteractor> list;
        private readonly ColorTheme _colorTheme;
        private readonly IProvideNodes _nodeProvider;

        public DiagramViewModel(
            DiagramModel diagram, 
            IProvideNodes nodeProvider, 
            ColorTheme colorTheme, 
            NodeSelectorViewModel nodeSelectorViewModel,
            IEnumerable<DiagramInteractor> diagramInteractors)
        {
            TerminalViewModel.ColorTheme = colorTheme;
            _colorTheme = colorTheme;
            _nodeProvider = nodeProvider ?? throw new ArgumentNullException(nameof(nodeProvider));

            DiagramInteractionManager = new DiagramInteractionManager(() => diagramInteractors);

            Diagram = diagram;
            Diagram.PropertyChanged += DiagramOnPropertyChanged;
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
        public DiagramModel Diagram { get; }
        public DiagramInteractionManager DiagramInteractionManager { get; set; }
        public string Name => Diagram.Name;
        public bool NodeBeingDragged { get; set; }
        public BindableCollection<PluginNode> NodeViewModels { get; set; } = new BindableCollection<PluginNode>();
        public BindableCollection<WireViewModel> WireViewModels { get; set; } = new BindableCollection<WireViewModel>();
        public BindableCollection<DiagramInteractor> ActiveDiagramInteractors { get; set; } = new BindableCollection<DiagramInteractor>();
        public double PanX { get; set; }
        public double PanY { get; set; }
        public double Zoom { get; set; } = 1;
        public double ViewWidth { get; set; }
        public double ViewHeight { get; set; }

        public void KeyDownHandler(object sender, KeyEventArgs e)
        {
            var interaction = new DiagramInteractionEventArguments();
            interaction.Type = InteractionType.KeyDown;
            interaction.Key = e.Key;
            DiagramInputHandler(interaction);
        }

        public void KeyUpHandler(object sender, KeyEventArgs e)
        {
            var interaction = new DiagramInteractionEventArguments();
            interaction.Type = InteractionType.KeyUp;
            interaction.Key = e.Key;
            DiagramInputHandler(interaction);
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

        private void MouseInputHandler(object sender, MouseButtonEventArgs e, InteractionType interactionType)
        {
            var relativeMousePosition = GetMousePositionRelativeToSender(sender, e);
            var interaction = new DiagramInteractionEventArguments();
            interaction.Type = interactionType;
            interaction.MousePosition = relativeMousePosition;
            DiagramInputHandler(interaction);
        }

        public void MouseWheelHandler(object sender, MouseWheelEventArgs e)
        {
            var relativeMousePosition = e.GetPosition((IInputElement)sender);
            var interaction = new DiagramInteractionEventArguments();
            interaction.Type = InteractionType.MouseWheel;
            interaction.MousePosition = relativeMousePosition;
            interaction.MouseWheelDelta = e.Delta;
            DiagramInputHandler(interaction);
        }

        public void PreviewMouseMoveHandler(object sender, MouseEventArgs e)
        {
            var relativeMousePosition = e.GetPosition((IInputElement)sender);
            var interaction = new DiagramInteractionEventArguments();
            interaction.Type = InteractionType.MouseMoved;
            interaction.MousePosition = relativeMousePosition;
            DiagramInputHandler(interaction);
        }

        private static Point GetMousePositionRelativeToSender(object sender, MouseButtonEventArgs e)
        {
            var inputElement = (IInputElement)sender;
            return e.GetPosition(inputElement);
        }

        public void AddNode(PluginNode viewModel)
        {
            if (viewModel.NodeModel == null)
            {
                throw new InvalidOperationException("Can not add a node to the diagram before it has been initialized");
            }
            Diagram.AddNode(viewModel.NodeModel);
            AddNodeViewModel(viewModel);
            viewModel.TerminalWiringModeChanged += PluginNodeOnWiringModeChanged;

            var interaction = new DiagramInteractionEventArguments();
            interaction.Type = InteractionType.NodeInserted;
            DiagramInputHandler(interaction);
        }

        private void AddNodeViewModel(PluginNode viewModel)
        {
            if (!Diagram.Nodes.Contains(viewModel.NodeModel))
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

        private void AddWiresForNode(PluginNode viewModel)
        {
            foreach (var inputTerminal in viewModel.NodeModel.Terminals.Where(t => t.Kind == TerminalKind.Input))
            {
                inputTerminal.ConnectedWires.ForEach(AddWireViewModel);
            }
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

        private void DiagramOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(Diagram.Name)))
            {
                NotifyOfPropertyChange(() => Name);
            }
        }

        public double GetPointRelativeToPanAndZoomX(double x)
        {
            return Zoom == 0 ? x : (x - PanX) / Zoom;
        }

        public double GetPointRelativeToPanAndZoomY(double y)
        {
            return Zoom == 0 ? y : (y - PanY) / Zoom;
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

        private void PluginNodeOnWiringModeChanged(TerminalViewModel terminalViewModel, bool enabled)
        {
            UnHighlightAllTerminals();
            UnselectNodes();
            if (enabled)
            {
                HighlightTerminalsOfSameType(terminalViewModel.TerminalModel);
            }
        }

        public void RemoveNode(PluginNode viewModel)
        {
            Diagram.RemoveNode(viewModel.NodeModel);
            NodeViewModels.Remove(viewModel);
            viewModel.TerminalWiringModeChanged -= PluginNodeOnWiringModeChanged;
            viewModel.DisconnectAllTerminals();
            viewModel.Uninitialize();
            UpdateDiagramBoundingBox();
            BoundingBoxVisible = NodeViewModels.Any();
            NotifyOfPropertyChange(nameof(AreInstructionsVisible));
        }

        private void UnHighlightAllTerminals()
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

        private void UpdateDiagramBoundingBox()
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
            if (!WireViewModels.Any(x => x.WireModel == wireModel))
            {
                AddWireViewModel(wireModel);
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

        private void DiagramInputHandler(DiagramInteractionEventArguments interaction)
        {
            DiagramInteractionManager.DiagramInputHandler(interaction, this);
            //interaction.Diagram = this;
            //PutViewModelMouseIsOverInInteraction(interaction);
            //PutKeyStatesInInteraction(interaction);
            //SendInteractionToInteractors(interaction);
        }

        private static void PutViewModelMouseIsOverInInteraction(DiagramInteractionEventArguments interaction)
        {
            var elementMouseIsOver = Mouse.DirectlyOver as FrameworkElement;
            if (!(elementMouseIsOver?.DataContext is Screen viewModelMouseIsOver))
            {
                var contentPresenter = elementMouseIsOver?.DataContext as ContentPresenter;
                viewModelMouseIsOver = contentPresenter?.DataContext as Screen;
            }
            interaction.ViewModelMouseIsOver = viewModelMouseIsOver;
        }

        private static void PutKeyStatesInInteraction(DiagramInteractionEventArguments interaction)
        {
            interaction.IsShiftKeyPressed = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.LeftShift);
            interaction.IsCtrlKeyPressed = Keyboard.IsKeyDown(Key.RightCtrl) || Keyboard.IsKeyDown(Key.LeftCtrl);
            interaction.IsAltKeyPressed = Keyboard.IsKeyDown(Key.RightAlt) || Keyboard.IsKeyDown(Key.LeftAlt);
        }

        private void SendInteractionToInteractors(DiagramInteractionEventArguments interaction)
        {
            if (!ActiveDiagramInteractors.Any())
            {
                StartInteractionsThatShouldStart(interaction);
            }
            SendInteractionToActiveInteractions(interaction);
            StopActiveInteractionsThatShouldStop(interaction);
        }

        private void StopActiveInteractionsThatShouldStop(DiagramInteractionEventArguments interaction)
        {
            var activeInteractors = ActiveDiagramInteractors.ToArray();
            foreach (var activeInteractor in activeInteractors)
            {
                if (activeInteractor.ShouldStopInteraction(interaction))
                {
                    activeInteractor.StopInteraction(interaction);
                    ActiveDiagramInteractors.Remove(activeInteractor);
                }
            }
        }

        private void SendInteractionToActiveInteractions(DiagramInteractionEventArguments interaction)
        {
            var activeInteractors = ActiveDiagramInteractors.ToArray();
            foreach (var activeInteractor in activeInteractors)
            {
                activeInteractor.ProcessInteraction(interaction);
            }
        }

        private void StartInteractionsThatShouldStart(DiagramInteractionEventArguments interaction)
        {
            //foreach (var interactor in DiagramInteractors)
            //{
            //    if (interactor.ShouldStartInteraction(interaction))
            //    {
            //        interactor.StartInteraction(interaction);
            //        ActiveDiagramInteractors.Add(interactor);
            //    }
            //}
        }
    }
}
