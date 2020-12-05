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
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;

namespace DiiagramrAPI.Editor.Interactors
{
    /// <summary>
    /// A palette that allows the user to find and pick a node to create a new instance of.
    /// </summary>
    public class NodePalette : DiagramInteractor
    {
        /// <summary>
        /// Whether or not to try to automatically wire to the context node when a new node is placed.
        /// </summary>
        public const bool AutoWireToContext = true;

        /// <summary>
        /// Defines how to sort nodes in the palette.
        /// </summary>
        public static bool ShouldSortNodesByLibraryInsteadOfByCategory;

        private const double NodeSelectorBottomMargin = 250;
        private const double NodeSelectorRightMargin = 400;
        private readonly INodeProvider _nodeProvider;
        private readonly DialogHostBase _dialogHost;
        private Diagram _diagram;
        private Func<Node, bool> _filter = x => true;

        /// <summary>
        /// Creates a new instance of <see cref="NodePalette"/>.
        /// </summary>
        /// <param name="nodeProviderFactory">A factory that returns an instance of <see cref="INodeProvider"/>.</param>
        /// <param name="dialogHostFactory">A factory that returns an instance of <see cref="DialogHostBase"/>.</param>
        public NodePalette(Func<INodeProvider> nodeProviderFactory, Func<DialogHostBase> dialogHostFactory)
        {
            _nodeProvider = nodeProviderFactory();
            _dialogHost = dialogHostFactory();
            if (_nodeProvider is NodeProvider provider)
            {
                provider.NodeRegistered += NodeProviderNodeRegistered;
            }
            AddNodes();
        }

        /// <summary>
        /// Gets the list of all possible nodes.
        /// </summary>
        public IEnumerable<Node> AvailableNodes => CategoriesList.SelectMany(l => l.Nodes);

        /// <summary>
        /// Gets or sets the context terminal. This is the terminal that the user last right clicked on.
        /// </summary>
        public Terminal ContextTerminal { get; set; }

        /// <summary>
        /// Gets the list of categories in the palette.
        /// </summary>
        public List<NodePaletteCategory> CategoriesList { get; } = new List<NodePaletteCategory>();

        /// <summary>
        /// Gets the node that the mouse is over.
        /// </summary>
        public Node MousedOverNode { get; set; }

        /// <summary>
        /// Gets the list of visible categories in the palette.
        /// </summary>
        public BindableCollection<NodePaletteCategory> VisibleCategoriesList { get; } = new BindableCollection<NodePaletteCategory>();

        /// <summary>
        /// Gets the list of visible nodes in the palette. These are nodes in the currently selected category.
        /// </summary>
        public BindableCollection<Node> VisibleNodesList { get; } = new BindableCollection<Node>();

        /// <summary>
        /// Populate the palette with nodes.
        /// </summary>
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

        /// <summary>
        /// Occurs when the user clicks anywhere off of the palette.
        /// </summary>
        public void BackgroundMouseDown()
        {
            VisibleNodesList.Clear();
            MousedOverNode = null;
        }

        /// <summary>
        /// Begin inserting a node onto the diagram.
        /// </summary>
        /// <param name="node">The node to insert.</param>
        /// <param name="insertCopy">Whether to insert a copy of <see cref="node"/>.</param>
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

        /// <summary>
        /// Occurs when the mouse enters a palette category list item.
        /// </summary>
        /// <param name="sender">The category list item that was entered.</param>
        /// <param name="e">The event arguments.</param>
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

        /// <summary>
        /// Occurs when the mouse has left the palette area.
        /// </summary>
        public void MouseLeftSelector()
        {
            CategoriesList.ForEach(l => l.UnselectCategoryItem());
            VisibleNodesList.Clear();
            MousedOverNode = null;
        }

        /// <summary>
        /// Occurs when the mouse enters a node list item.
        /// </summary>
        /// <param name="sender">The node list item the mouse has entered.</param>
        /// <param name="e">The event arguments.</param>
        public void NodeMouseEnterHandler(object sender, MouseEventArgs e)
        {
            if (!(((Border)sender).DataContext is Node node))
            {
                return;
            }
            MousedOverNode = VisibleNodesList.First(m => m.Name == node.Name);
        }

        /// <inheritdoc/>
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

        /// <summary>
        /// Occurs when the user clicks on a node list item.
        /// </summary>
        public void SelectNode()
        {
            BeginInsertingNode(MousedOverNode, true);
        }

        /// <inheritdoc/>
        public override bool ShouldStartInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.RightMouseDown;
        }

        /// <inheritdoc/>
        public override bool ShouldStopInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.LeftMouseDown || interaction.Type == InteractionType.NodeInserted;
        }

        public void ShowCategory(NodePaletteCategory category)
        {
            VisibleNodesList.Clear();

            new Thread(() =>
            {
                foreach (var node in category.Nodes.Where(_filter).OrderBy(n => n.Weight))
                {
                    _diagram.View?.Dispatcher.Invoke(() => VisibleNodesList.Add(node));
                    Thread.Sleep(10);
                }
            }).Start();

            VisibleCategoriesList.ForEach(l => l.UnselectCategoryItem());
            category.SelectCategoryItem();
            MousedOverNode = null;
        }

        /// <inheritdoc/>
        public override void StartInteraction(DiagramInteractionEventArguments interaction)
        {
        }

        /// <inheritdoc/>
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
            var nodeModel = new NodeModel(node.Name);
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
            var firstCategoryAdded = false;
            VisibleCategoriesList.Clear();
            var loadingCategory = new NodePaletteCategory("...");
            VisibleCategoriesList.Add(loadingCategory);

            new Thread(() =>
            {
                foreach (var category in CategoriesList)
                {
                    foreach (var node in category.Nodes)
                    {
                        if (filter(node))
                        {
                            _diagram.View?.Dispatcher.Invoke(() => VisibleCategoriesList.Add(category));
                            if (!firstCategoryAdded) _diagram.View?.Dispatcher.Invoke(() => VisibleCategoriesList.Remove(loadingCategory));
                            firstCategoryAdded = true;
                            Thread.Sleep(10);
                            break;
                        }
                    }
                }
                if (!firstCategoryAdded) _diagram.View?.Dispatcher.Invoke(() => VisibleCategoriesList.Remove(loadingCategory));
            }).Start();
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