using DiiagramrAPI.Service;
using DiiagramrAPI.Service.Interfaces;
using DiiagramrAPI.Shell.Commands.Transacting;
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
        private IProvideNodes _nodeProvider;
        private Diagram _diagram;
        private bool nodesAdded = false;

        public NodePalette(Func<IProvideNodes> nodeProvider)
        {
            _nodeProvider = nodeProvider.Invoke();
            _nodeProvider.PropertyChanged += NodesOnPropertyChanged;
            AddNodes();
        }

        public Terminal ContextTerminal { get; set; }
        public IEnumerable<Node> AvailableNodes => LibrariesList.SelectMany(l => l.Nodes);
        public List<NodePaletteLibrary> LibrariesList { get; set; } = new List<NodePaletteLibrary>();
        public BindableCollection<NodePaletteLibrary> VisibleLibrariesList { get; set; } = new BindableCollection<NodePaletteLibrary>();
        public Node MousedOverNode { get; set; }
        public bool NodePreviewVisible => MousedOverNode != null;
        public double PreviewNodePositionX { get; set; }
        public double PreviewNodePositionY { get; set; }
        public double PreviewNodeScaleX { get; set; }
        public double PreviewNodeScaleY { get; set; }

        public BindableCollection<Node> VisibleNodesList { get; set; } = new BindableCollection<Node>();

        private Func<Node, bool> Filter = x => true;

        public void AddNodes()
        {
            if (nodesAdded)
            {
                return;
            }
            nodesAdded = true;

            foreach (var Node in _nodeProvider.GetRegisteredNodes())
            {
                if (CanAddNodeToPalette(Node))
                {
                    TryAddingNode(Node);
                }
            }
        }

        private bool CanAddNodeToPalette(Node node)
        {
            if (IsHiddenFromSelector(node))
            {
                return false;
            }
            var library = GetOrCreateLibrary(node);
            return !library.Nodes.Any(n => n.Equals(node));
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
            }
        }

        private void AddNode(Node node)
        {
            NodeModel nodeModel = new NodeModel("");
            // Required to get the terminal data. Ideally this should not be required in case nodes initialize a lot when they are initialized with a model.
            node.AttachToModel(nodeModel);
            var library = GetOrCreateLibrary(node);
            library.Nodes.Add(node);
        }

        public void BackgroundMouseDown()
        {
            VisibleNodesList.Clear();
            MousedOverNode = null;
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
            LibrariesList.ForEach(l => l.Unselect());
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

        public void SelectNode()
        {
            BeginInsertingNode(MousedOverNode, true);
        }

        public void BeginInsertingNode(Node node, bool insertCopy = false)
        {
            var nodeTypeName = node.GetType().FullName;
            var nodeToInsert = insertCopy ? _nodeProvider.CreateNodeFromName(nodeTypeName) : node;
            nodeToInsert.Visible = false;
            nodeToInsert.Model.X = _diagram.GetDiagramPointFromViewPointX(X);
            nodeToInsert.Model.Y = _diagram.GetDiagramPointFromViewPointX(Y);
            _diagram.AddNodeInteractively(nodeToInsert);
            AutoWireTerminals(nodeToInsert);
            ContextTerminal = null;
        }

        private void AutoWireTerminals(Node nodeToInsert)
        {
            if (ContextTerminal != null)
            {
                var terminalsThatCouldBeWired = GetWireableTerminals(ContextTerminal, nodeToInsert);
                if (terminalsThatCouldBeWired.Count() == 1)
                {
                    TerminalWirer.TryWireTwoTerminalsOnDiagram(_diagram, ContextTerminal, terminalsThatCouldBeWired.First(), NullTransactor.Instance, false);
                }
            }
        }

        private IEnumerable<Terminal> GetWireableTerminals(Terminal startTerminal, Node node)
        {
            if (startTerminal.Model is InputTerminalModel inputTerminal)
            {
                return node.Terminals
                    .OfType<OutputTerminal>()
                    .Where(t => t.Model.Type.IsAssignableFrom(inputTerminal.Type));

            }
            else if (startTerminal.Model is OutputTerminalModel outputTerminal)
            {
                return node.Terminals
                    .OfType<InputTerminal>()
                    .Where(t => t.Model.Type.IsAssignableFrom(outputTerminal.Type));
            }
            return Enumerable.Empty<Terminal>();
        }

        public void ShowLibrary(NodePaletteLibrary library)
        {
            VisibleNodesList.Clear();
            VisibleNodesList.AddRange(library.Nodes.Where(Filter).OrderBy(n => n.Weight));
            VisibleLibrariesList.ForEach(l => l.Unselect());
            library.Select();
            MousedOverNode = null;
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
            return node.GetType().IsDefined(typeof(HideFromNodeSelector), false);
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

        public override bool ShouldStartInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.RightMouseDown;
        }

        public override bool ShouldStopInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.LeftMouseDown || interaction.Type == InteractionType.NodeInserted;
        }

        public override void StartInteraction(DiagramInteractionEventArguments interaction)
        {
        }

        private void ShowWithContextFilter(Screen mousedOverViewModel)
        {
            if (mousedOverViewModel is InputTerminal inputTerminalMouseIsOver)
            {
                ContextTerminal = inputTerminalMouseIsOver;
                Show(n => n.Terminals.Any(t => t is OutputTerminal && t.Model.Type.IsAssignableFrom(inputTerminalMouseIsOver.Model.Type)));
                inputTerminalMouseIsOver.HighlightVisible = true;
            }
            else if (mousedOverViewModel is OutputTerminal outputTerminalMouseIsOver)
            {
                ContextTerminal = outputTerminalMouseIsOver;
                Show(n => n.Terminals.Any(t => t is InputTerminal && t.Model.Type.IsAssignableFrom(outputTerminalMouseIsOver.Model.Type)));
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

        private void Show(Func<Node, bool> filter)
        {
            Filter = filter;
            VisibleLibrariesList.Clear();
            VisibleLibrariesList.AddRange(LibrariesList.Where(l => l.Nodes.Where(filter).Any()));
        }

        public override void StopInteraction(DiagramInteractionEventArguments interaction)
        {
            if (ContextTerminal != null)
            {
                ContextTerminal.HighlightVisible = false;
            }
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
                    if (terminal.Model.DefaultSide != Direction.West)
                    {
                        X += Terminal.TerminalHeight;
                    }
                }
                ShowWithContextFilter(viewModelUnderMouse);
            }
        }
    }
}
