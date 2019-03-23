using DiiagramrAPI.Diagram.Model;
using DiiagramrAPI.Diagram.Nodes;
using DiiagramrAPI.Service;
using DiiagramrAPI.Service.Interfaces;
using Stylet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace DiiagramrAPI.Diagram.Interactors
{
    public class NodePalette : DiagramInteractor
    {
        public const bool AutoWireToContext = true;
        private const double NodeSelectorBottomMargin = 250;
        private const double NodeSelectorRightMargin = 400;
        private IProvideNodes _nodeProvider;
        private Diagram _diagramViewModel;
        private bool nodesAdded = false;

        public NodePalette(Func<IProvideNodes> nodeProvider)
        {
            _nodeProvider = nodeProvider.Invoke();
            _nodeProvider.PropertyChanged += NodesOnPropertyChanged;
            AddNodes();
        }

        public Terminal ContextTerminal { get; set; }
        public IEnumerable<Node> AvailableNodeViewModels => LibrariesList.SelectMany(l => l.Nodes);
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
            foreach (var nodeViewModel in _nodeProvider.GetRegisteredNodes())
            {
                if (nodeViewModel is DiagramCallNode)
                {
                    continue;
                }

                if (IsHiddenFromSelector(nodeViewModel))
                {
                    continue;
                }

                var fullTypeName = nodeViewModel.GetType().FullName;
                var libraryName = fullTypeName?.Split('.').FirstOrDefault() ?? fullTypeName;
                var library = GetOrCreateLibrary(libraryName);
                if (library.Nodes.Any(n => n.Equals(nodeViewModel)))
                {
                    continue;
                }

                var nodeModel = new NodeModel("");
                try
                {
                    nodeViewModel.InitializeWithNode(nodeModel);
                    library.Nodes.Add(nodeViewModel);
                }
                catch (FileNotFoundException e)
                {
                    Console.Error.WriteLine($"Error in '{fullTypeName}.InitializeWithNode(NodeModel node)' --- Exception message: {e.Message}");
                }
            }
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
            var nodeToInsert = insertCopy ? _nodeProvider.CreateNodeViewModelFromName(nodeTypeName) : node;
            AutoWireTerminals(nodeToInsert);
            nodeToInsert.Visible = false;
            nodeToInsert.Model.X = _diagramViewModel.GetDiagramPointFromViewPointX(X);
            nodeToInsert.Model.Y = _diagramViewModel.GetDiagramPointFromViewPointX(Y);
            _diagramViewModel.AddNode(nodeToInsert);
            ContextTerminal = null;
        }

        private void AutoWireTerminals(Node nodeToInsert)
        {
            if (ContextTerminal == null) return;
            var terminalsThatCouldBeWired = GetWireableTerminals(ContextTerminal, nodeToInsert);
            if (terminalsThatCouldBeWired.Count() == 1)
            {
                ContextTerminal.WireToTerminal(terminalsThatCouldBeWired.First().Model);
            }
        }

        private IEnumerable<Terminal> GetWireableTerminals(Terminal startTerminal, Node node)
        {
            if (startTerminal.Model.Kind == TerminalKind.Input)
            {
                return node.TerminalViewModels
                    .Where(t => t is OutputTerminal
                             && t.Model.Type.IsAssignableFrom(startTerminal.Model.Type));

            }
            else if (startTerminal.Model.Kind == TerminalKind.Output)
            {
                return node.TerminalViewModels
                    .Where(t => t is InputTerminal
                        && t.Model.Type.IsAssignableFrom(startTerminal.Model.Type));
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

        private NodePaletteLibrary GetOrCreateLibrary(string libraryName)
        {
            if (LibrariesList.All(l => l.Name != libraryName))
            {
                LibrariesList.Insert(0, new NodePaletteLibrary(libraryName));
            }

            return LibrariesList.First(l => l.Name == libraryName);
        }

        private bool IsHiddenFromSelector(Node nodeViewModel)
        {
            return nodeViewModel.GetType().IsDefined(typeof(HideFromNodeSelector), false);
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
            var totalNodeWidth = MousedOverNode.Width + Diagram.NodeBorderWidth * 2;
            var totalNodeHeight = MousedOverNode.Height + Diagram.NodeBorderWidth * 2;
            PreviewNodeScaleX = workingWidth / totalNodeWidth;
            PreviewNodeScaleY = workingHeight / totalNodeHeight;

            PreviewNodeScaleX = Math.Min(PreviewNodeScaleX, PreviewNodeScaleY);
            PreviewNodeScaleY = Math.Min(PreviewNodeScaleX, PreviewNodeScaleY);

            var newWidth = totalNodeWidth * PreviewNodeScaleX;
            var newHeight = totalNodeHeight * PreviewNodeScaleY;

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
                Show(n => n.TerminalViewModels.Any(t => t is OutputTerminal && t.Model.Type.IsAssignableFrom(inputTerminalMouseIsOver.Model.Type)));
                inputTerminalMouseIsOver.HighlightVisible = true;
            }
            else if (mousedOverViewModel is OutputTerminal outputTerminalMouseIsOver)
            {
                ContextTerminal = outputTerminalMouseIsOver;
                Show(n => n.TerminalViewModels.Any(t => t is InputTerminal && t.Model.Type.IsAssignableFrom(outputTerminalMouseIsOver.Model.Type)));
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
                _diagramViewModel = interaction.Diagram;

                var availableWidth = _diagramViewModel.View != null ? _diagramViewModel.View.RenderSize.Width : 0;
                var availableHeight = _diagramViewModel.View != null ? _diagramViewModel.View.RenderSize.Height : 0;
                X = Math.Min(interaction.MousePosition.X, availableWidth - NodeSelectorRightMargin);
                Y = Math.Min(interaction.MousePosition.Y, availableHeight - NodeSelectorBottomMargin);

                var mousedOverViewModel = interaction.ViewModelMouseIsOver;

                if (mousedOverViewModel is Terminal terminal)
                {
                    if (terminal.Model.Direction != Direction.West)
                    {
                        X += Terminal.TerminalDiameter;
                    }
                }
                ShowWithContextFilter(mousedOverViewModel);
            }
        }
    }
}
