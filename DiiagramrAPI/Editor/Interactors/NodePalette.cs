using DiiagramrAPI.Editor.Diagrams;
using DiiagramrAPI.Service.Editor;
using DiiagramrCore;
using DiiagramrModel;
using Stylet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace DiiagramrAPI.Editor.Interactors
{
    public class NodePalette : DiagramInteractor
    {
        public const bool AutoWireToContext = true;
        private const double NodeSelectorBottomMargin = 250;
        private const double NodeSelectorRightMargin = 400;
        private readonly IProvideNodes _nodeProvider;
        private Diagram _diagram;
        private Func<Node, bool> filter = x => true;

        public NodePalette(Func<IProvideNodes> nodeProvider)
        {
            _nodeProvider = nodeProvider();
            _nodeProvider.PropertyChanged += NodesOnPropertyChanged;
            AddNodes();
        }

        public IEnumerable<Node> AvailableNodes => LibrariesList.SelectMany(l => l.Nodes);

        public Terminal ContextTerminal { get; set; }

        public List<NodePaletteLibrary> LibrariesList { get; } = new List<NodePaletteLibrary>();

        public Node MousedOverNode { get; set; }

        public bool NodePreviewVisible => MousedOverNode != null;

        public double PreviewNodePositionX { get; set; }

        public double PreviewNodePositionY { get; set; }

        public double PreviewNodeScaleX { get; set; }

        public double PreviewNodeScaleY { get; set; }

        public BindableCollection<NodePaletteLibrary> VisibleLibrariesList { get; } = new BindableCollection<NodePaletteLibrary>();

        public BindableCollection<Node> VisibleNodesList { get; } = new BindableCollection<Node>();

        public void AddNodes()
        {
            foreach (var node in _nodeProvider.GetRegisteredNodes())
            {
                if (CanAddNodeToPalette(node))
                {
                    TryAddingNode(node);
                }
            }
        }

        public void BackgroundMouseDown()
        {
            VisibleNodesList.Clear();
            MousedOverNode = null;
        }

        public void BeginInsertingNode(Node node, bool insertCopy = false)
        {
            var nodeTypeName = node.GetType().FullName;
            var nodeToInsert = insertCopy ? _nodeProvider.CreateNodeFromName(nodeTypeName) : node;
            nodeToInsert.Visible = false;
            nodeToInsert.NodeModel.X = _diagram.GetDiagramPointFromViewPointX(X);
            nodeToInsert.NodeModel.Y = _diagram.GetDiagramPointFromViewPointX(Y);
            _diagram.AddNodeInteractively(nodeToInsert);
            if (ContextTerminal != null)
            {
                var autoWirer = new NodeAutoWirer();
                autoWirer.TryAutoWireTerminals(_diagram, ContextTerminal, nodeToInsert);
            }
            ContextTerminal = null;
        }

        public void LibraryMouseEnterHandler(object sender, MouseEventArgs e)
        {
            if (!(((Border)sender).DataContext is NodePaletteLibrary library))
            {
                return;
            }

            if (!library.NodesLoaded)
            {
                library.NodesLoaded = true;
            }

            ShowLibrary(library);
        }

        public void MouseLeftSelector()
        {
            LibrariesList.ForEach(l => l.UnselectLibraryItem());
            VisibleNodesList.Clear();
            MousedOverNode = null;
        }

        public void NodeMouseEnterHandler(object sender, MouseEventArgs e)
        {
            if (!(((Border)sender).DataContext is Node node))
            {
                return;
            }

            PreviewNode(node);
        }

        public override void ProcessInteraction(DiagramInteractionEventArguments interaction)
        {
            if (interaction.Type == InteractionType.RightMouseDown)
            {
                _diagram = interaction.Diagram;

                var availableWidth = _diagram.View != null ? _diagram.View.RenderSize.Width : 0;
                var availableHeight = _diagram.View != null ? _diagram.View.RenderSize.Height : 0;
                X = Math.Min(interaction.MousePosition.X, availableWidth - NodeSelectorRightMargin);
                Y = Math.Min(interaction.MousePosition.Y, availableHeight - NodeSelectorBottomMargin);

                var viewModelUnderMouse = interaction.ViewModelUnderMouse;

                if (viewModelUnderMouse is Terminal terminal)
                {
                    if (terminal.TerminalModel.DefaultSide != Direction.West)
                    {
                        X += Terminal.TerminalHeight;
                    }
                }

                ShowWithContextFilter(viewModelUnderMouse);
            }
        }

        public void SelectNode()
        {
            BeginInsertingNode(MousedOverNode, true);
        }

        public override bool ShouldStartInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.RightMouseDown;
        }

        public override bool ShouldStopInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.LeftMouseDown || interaction.Type == InteractionType.NodeInserted;
        }

        public void ShowLibrary(NodePaletteLibrary library)
        {
            VisibleNodesList.Clear();
            VisibleNodesList.AddRange(library.Nodes.Where(filter).OrderBy(n => n.Weight));
            VisibleLibrariesList.ForEach(l => l.UnselectLibraryItem());
            library.SelectLibraryItem();
            MousedOverNode = null;
        }

        public override void StartInteraction(DiagramInteractionEventArguments interaction)
        {
        }

        public override void StopInteraction(DiagramInteractionEventArguments interaction)
        {
            if (ContextTerminal != null)
            {
                ContextTerminal.HighlightVisible = false;
            }
        }

        private void AddNode(Node node)
        {
            NodeModel nodeModel = new NodeModel(string.Empty);

            // Required to get the terminal data. Ideally this should not be required in case nodes initialize a lot when they are initialized with a model.
            node.AttachToModel(nodeModel);
            var library = GetOrCreateLibrary(node);
            library.Nodes.Add(node);
        }

        private bool CanAddNodeToPalette(Node node)
        {
            if (AvailableNodes.Any(n => n.Name == node.Name))
            {
                return false;
            }
            if (IsHiddenFromSelector(node))
            {
                return false;
            }

            var library = GetOrCreateLibrary(node);
            return !library.Nodes.Any(n => n.Equals(node));
        }

        private NodePaletteLibrary GetOrCreateLibrary(Node node)
        {
            var fullTypeName = node.GetType().FullName;
            var libraryName = fullTypeName?.Split('.').FirstOrDefault() ?? fullTypeName;
            return GetOrCreateLibrary(libraryName);
        }

        private NodePaletteLibrary GetOrCreateLibrary(string libraryName)
        {
            if (LibrariesList.All(l => l.Name != libraryName))
            {
                LibrariesList.Insert(0, new NodePaletteLibrary(libraryName));
            }

            return LibrariesList.First(l => l.Name == libraryName);
        }

        private bool IsHiddenFromSelector(Node node)
        {
            return node.GetType().IsDefined(typeof(HideFromNodeSelectorAttribute), false);
        }

        private void NodesOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            AddNodes();
        }

        private void PreviewNode(Node node)
        {
            const int workingWidth = 100;
            const int workingHeight = 100;

            MousedOverNode = VisibleNodesList.First(m => m.Name == node.Name);
            var totalNodeWidth = MousedOverNode.Width + Diagram.NodeBorderWidth * 2.0;
            var totalNodeHeight = MousedOverNode.Height + Diagram.NodeBorderWidth * 2.0;
            PreviewNodeScaleX = workingWidth / totalNodeWidth;
            PreviewNodeScaleY = workingHeight / totalNodeHeight;

            PreviewNodeScaleX = Math.Min(PreviewNodeScaleX, PreviewNodeScaleY);
            PreviewNodeScaleY = Math.Min(PreviewNodeScaleX, PreviewNodeScaleY);

            var newWidth = totalNodeWidth * PreviewNodeScaleX + 1;
            var newHeight = totalNodeHeight * PreviewNodeScaleY + 1;

            PreviewNodePositionX = (workingWidth - newWidth) / 2.0;
            PreviewNodePositionY = (workingHeight - newHeight) / 2.0;
        }

        private void Show(Func<Node, bool> filter)
        {
            this.filter = filter;
            VisibleLibrariesList.Clear();
            VisibleLibrariesList.AddRange(LibrariesList.Where(l => l.Nodes.Where(filter).Any()));
        }

        private void ShowWithContextFilter(Screen mousedOverViewModel)
        {
            if (mousedOverViewModel is InputTerminal inputTerminalMouseIsOver)
            {
                ContextTerminal = inputTerminalMouseIsOver;
                Show(n => n.Terminals.Any(t => t is OutputTerminal && ValueConverter.NonExaustiveCanConvertToType(t.TerminalModel.Type, inputTerminalMouseIsOver.TerminalModel.Type)));
                inputTerminalMouseIsOver.HighlightVisible = true;
            }
            else if (mousedOverViewModel is OutputTerminal outputTerminalMouseIsOver)
            {
                ContextTerminal = outputTerminalMouseIsOver;
                Show(n => n.Terminals.Any(t => t is InputTerminal && ValueConverter.NonExaustiveCanConvertToType(outputTerminalMouseIsOver.TerminalModel.Type, t.TerminalModel.Type)));
                outputTerminalMouseIsOver.HighlightVisible = true;
            }
            else
            {
                Show(n => true);
            }

            if (ContextTerminal != null)
            {
                ContextTerminal.SetAdorner(null);
            }
        }

        private void TryAddingNode(Node node)
        {
            try
            {
                AddNode(node);
            }
            // TODO: Catch more specific exception.
            catch (Exception e)
            {
                Console.Error.WriteLine($"Error in '{node.GetType().FullName}.InitializeWithNode(NodeModel node)' --- Exception message: {e.Message}");
                throw;
            }
        }
    }
}