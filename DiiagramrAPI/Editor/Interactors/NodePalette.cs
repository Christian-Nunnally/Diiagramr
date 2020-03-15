using DiiagramrAPI.Application;
using DiiagramrAPI.Application.Dialogs;
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
        public static bool ShouldSortNodesByLibraryInsteadOfByCategory;
        private const double NodeSelectorBottomMargin = 250;
        private const double NodeSelectorRightMargin = 400;
        private readonly INodeProvider _nodeProvider;
        private readonly DialogHostBase _dialogHost;
        private Diagram _diagram;
        private Func<Node, bool> _filter = x => true;

        public NodePalette(
            Func<INodeProvider> nodeProviderFactory, Func<DialogHostBase> dialogHostFactory)
        {
            _nodeProvider = nodeProviderFactory();
            _dialogHost = dialogHostFactory();
            if (_nodeProvider is NodeProvider provider)
            {
                provider.NodeRegistered += NodeProviderNodeRegistered;
            }
            AddNodes();
        }

        public IEnumerable<Node> AvailableNodes => CategoriesList.SelectMany(l => l.Nodes);

        public Terminal ContextTerminal { get; set; }

        public List<NodePaletteCategory> CategoriesList { get; } = new List<NodePaletteCategory>();

        public Node MousedOverNode { get; set; }

        public BindableCollection<NodePaletteCategory> VisibleCategoriesList { get; } = new BindableCollection<NodePaletteCategory>();

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
            nodeToInsert.Model.X = _diagram.GetDiagramPointFromViewPointX(X);
            nodeToInsert.Model.Y = _diagram.GetDiagramPointFromViewPointX(Y);
            _diagram.AddNodeInteractively(nodeToInsert);
            if (ContextTerminal != null)
            {
                var autoWirer = new NodeAutoWirer();
                autoWirer.TryAutoWireTerminals(_diagram, ContextTerminal, nodeToInsert);
            }
            ContextTerminal = null;
        }

        public void CategoryMouseEnterHandler(object sender, MouseEventArgs e)
        {
            if (!(((Border)sender).DataContext is NodePaletteCategory category))
            {
                return;
            }

            if (!category.NodesLoaded)
            {
                category.NodesLoaded = true;
            }

            ShowCategory(category);
        }

        public void MouseLeftSelector()
        {
            CategoriesList.ForEach(l => l.UnselectCategoryItem());
            VisibleNodesList.Clear();
            MousedOverNode = null;
        }

        public void NodeMouseEnterHandler(object sender, MouseEventArgs e)
        {
            if (!(((Border)sender).DataContext is Node node))
            {
                return;
            }
            MousedOverNode = VisibleNodesList.First(m => m.Name == node.Name);
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
                OffsetPaletteIfClickingOnTerminal(viewModelUnderMouse);

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

        public void ShowCategory(NodePaletteCategory category)
        {
            VisibleNodesList.Clear();
            VisibleNodesList.AddRange(category.Nodes.Where(_filter).OrderBy(n => n.Weight));
            VisibleCategoriesList.ForEach(l => l.UnselectCategoryItem());
            category.SelectCategoryItem();
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

        private void OffsetPaletteIfClickingOnTerminal(Screen viewModelUnderMouse)
        {
            if (viewModelUnderMouse is Terminal terminal)
            {
                if (terminal.Model.DefaultSide != Direction.West)
                {
                    X += Terminal.TerminalHeight;
                }
            }
        }

        private void NodeProviderNodeRegistered(Node node)
        {
            if (CanAddNodeToPalette(node))
            {
                TryAddingNode(node);
            }
        }

        private void AddNode(Node node)
        {
            NodeModel nodeModel = new NodeModel(string.Empty);

            // Required to get the terminal data. Ideally this should not be required in case nodes initialize a lot when they are initialized with a model.
            node.AttachToModel(nodeModel);
            var category = GetOrCreateCategory(node);
            category.Nodes.Add(node);
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

            var category = GetOrCreateCategory(node);
            return !category.Nodes.Any(n => n.Equals(node));
        }

        private NodePaletteCategory GetOrCreateCategory(Node node)
        {
            var fullTypeName = node.GetType().FullName;
            var libraryName = fullTypeName?.Split('.').FirstOrDefault() ?? fullTypeName;
            return GetOrCreateCategory(libraryName);
        }

        private NodePaletteCategory GetOrCreateCategory(string categoryName)
        {
            if (CategoriesList.All(l => l.Name != categoryName))
            {
                CategoriesList.Insert(0, new NodePaletteCategory(categoryName));
            }

            return CategoriesList.First(l => l.Name == categoryName);
        }

        private bool IsHiddenFromSelector(Node node)
        {
            return node.GetType().IsDefined(typeof(HideFromNodeSelectorAttribute), false);
        }

        private void NodesOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            AddNodes();
        }

        private void Show(Func<Node, bool> filter)
        {
            _filter = filter;
            VisibleCategoriesList.Clear();
            VisibleCategoriesList.AddRange(CategoriesList.Where(l => l.Nodes.Where(filter).Any()));
        }

        private void ShowWithContextFilter(Screen mousedOverViewModel)
        {
            if (mousedOverViewModel is InputTerminal inputTerminalMouseIsOver)
            {
                ContextTerminal = inputTerminalMouseIsOver;
                Show(n => n.Terminals.Any(t => t is OutputTerminal && ValueConverter.TryCoerseValue(t.Data, inputTerminalMouseIsOver.Model.Type, out var _)));
                inputTerminalMouseIsOver.HighlightVisible = true;
            }
            else if (mousedOverViewModel is OutputTerminal outputTerminalMouseIsOver)
            {
                ContextTerminal = outputTerminalMouseIsOver;
                Show(n => n.Terminals.Any(t => t is InputTerminal && ValueConverter.TryCoerseValue(outputTerminalMouseIsOver.Data, t.Model.Type, out var _)));
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
            catch (Exception e)
            {
                var exceptionMessage = $"Error in '{node.GetType().FullName}.InitializeWithNode(NodeModel node)'-- - Exception message: { e.Message }";
                var messageBoxBuilder = new MessageBox.Builder("Error Adding Node", exceptionMessage).WithChoice("Ok", () => { });
                _dialogHost.OpenDialog(messageBoxBuilder.Build());
            }
        }
    }
}