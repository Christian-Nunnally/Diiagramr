using DiiagramrAPI.Editor.Diagrams;
using DiiagramrAPI.Editor.Interactors.TextTree;
using DiiagramrAPI.Service.Editor;
using DiiagramrCore;
using DiiagramrModel;
using PropertyChanged;
using Stylet;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DiiagramrAPI.Editor.Interactors
{
    /// <summary>
    /// A palette that allows users to search for nodes using a text box and then select one to create on the diagram.
    /// </summary>
    public class SearchPalette : DiagramInteractor
    {
        private const int MaxSearchResults = 10;
        private readonly INodeProvider _nodeProvider;
        private readonly FastTextSearchNode<SearchResult> _rootNode = new FastTextSearchNode<SearchResult>();
        private Diagram _diagram;
        private bool _shouldStopinteraction = false;
        private Node _nodeToInsert;
        private int _selectedNodeIndex = -1;

        /// <summary>
        /// Creates a new instance of <see cref="SearchPalette"/>.
        /// </summary>
        /// <param name="nodeProviderFactory">A factory that returns an instance of <see cref="INodeProvider"/>.</param>
        public SearchPalette(Func<INodeProvider> nodeProviderFactory)
        {
            Weight = 0;
            _nodeProvider = nodeProviderFactory();
            if (_nodeProvider is NodeProvider provider)
            {
                provider.NodeRegistered += ProviderNodeRegistered;
            }
            PropertyChanged += PropertyChangedHandler;
            AddResultsToSearchTree();
        }

        /// <summary>
        /// Gets or sets the search text the user has typed.
        /// </summary>
        public string SearchPhrase { get; set; } = string.Empty;

        /// <summary>
        /// Gets the list of nodes filtered by the <see cref="SearchPhrase"/>.
        /// </summary>
        public BindableCollection<SearchResult> FilteredNodesList { get; } = new BindableCollection<SearchResult>();

        /// <summary>
        /// Gets or gets the index of the selected node.
        /// </summary>
        public int SelectedNodeIndex
        {
            get => _selectedNodeIndex;
            set
            {
                if (_selectedNodeIndex >= 0 && _selectedNodeIndex < FilteredNodesList.Count)
                {
                    FilteredNodesList[_selectedNodeIndex].IsSelected = false;
                }
                _selectedNodeIndex = value;
                if (_selectedNodeIndex >= 0 && _selectedNodeIndex < FilteredNodesList.Count)
                {
                    FilteredNodesList[_selectedNodeIndex].IsSelected = true;
                    NodeToPreview = FilteredNodesList[_selectedNodeIndex].Node;
                }
                else
                {
                    NodeToPreview = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the node to show a visual preview of.
        /// </summary>
        public Node NodeToPreview { get; set; }

        /// <inheritdoc/>
        public override void ProcessInteraction(DiagramInteractionEventArguments interaction)
        {
        }

        /// <inheritdoc/>
        public override bool ShouldStartInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.KeyDown
                && interaction.Key == Key.Space
                && !interaction.IsModifierKeyPressed;
        }

        /// <inheritdoc/>
        public override bool ShouldStopInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.RightMouseDown
                || interaction.Type == InteractionType.LeftMouseDown
                || _shouldStopinteraction;
        }

        /// <summary>
        /// Occurs when the search text box loses keyboard focus.
        /// </summary>
        public void SearchTextBoxLostFocus()
        {
        }

        /// <summary>
        /// Occurs when the search box visual is loaded.
        /// </summary>
        /// <param name="sender">The search box visual.</param>
        /// <param name="e">The event arguments.</param>
        public void SearchTextBoxLoaded(object sender, RoutedEventArgs e)
        {
            FocusTextBox(sender as TextBox);
        }

        /// <summary>
        /// Occurs when the user types a key into the search box.
        /// </summary>
        /// <param name="sender">The search box visual.</param>
        /// <param name="e">The event arguments.</param>
        public void SearchTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                SelectedNodeIndex = Math.Min(FilteredNodesList.Count - 1, SelectedNodeIndex + 1);
            }
            else if (e.Key == Key.Up)
            {
                SelectedNodeIndex = Math.Max(-1, SelectedNodeIndex - 1);
            }
            else if (e.Key == Key.Enter)
            {
                var node = FilteredNodesList.FirstOrDefault(n => n.IsSelected);
                if (node != null)
                {
                    _shouldStopinteraction = true;
                    _nodeToInsert = node.Node;
                }
            }
            else if (e.Key == Key.Escape)
            {
                _shouldStopinteraction = true;
            }
        }

        /// <inheritdoc/>
        public override void StartInteraction(DiagramInteractionEventArguments interaction)
        {
            _shouldStopinteraction = false;
            _diagram = interaction.Diagram;
            X = interaction.MousePosition.X;
            Y = interaction.MousePosition.Y;
            SearchPhrase = string.Empty;
            SelectedNodeIndex = -1;
        }

        /// <inheritdoc/>
        public override void StopInteraction(DiagramInteractionEventArguments interaction)
        {
            if (_nodeToInsert != null)
            {
                BeginInsertingNode(_nodeToInsert, true);
            }
            _shouldStopinteraction = false;
            _nodeToInsert = null;
        }

        /// <summary>
        /// Occurs when then mouse enters a node list item.
        /// </summary>
        /// <param name="sender">The node list item that the mouse is over.</param>
        /// <param name="e">The event arguments.</param>
        public void NodeMouseEnterHandler(object sender, MouseEventArgs e)
        {
            if (!(((Border)sender).DataContext is SearchResult searchResult))
            {
                return;
            }

            FilteredNodesList.ForEach(n => n.IsSelected = false);
            searchResult.IsSelected = true;
            NodeToPreview = searchResult.Node;
        }

        /// <summary>
        /// Occurs when the user clicks on a node list item.
        /// </summary>
        public void SelectNode()
        {
            _shouldStopinteraction = true;
            BeginInsertingNode(NodeToPreview, true);
        }

        private void AddResultsToSearchTree()
        {
            foreach (var result in _nodeProvider
                .GetRegisteredNodes()
                .Select(n => new SearchResult() { Node = n, DisplayName = n.Name })
                .ToList())
            {
                AddAndInitializeNewNode(result);
            }
        }

        private void AddAndInitializeNewNode(SearchResult result)
        {
            NodeModel nodeModel = new NodeModel(string.Empty);
            // Required to get the terminal data. Ideally this should not be required in case nodes initialize a lot when they are initialized with a model.
            result.Node.AttachToModel(nodeModel);
            _rootNode.Add(result.SearchPhrase, result);
        }

        private void BeginInsertingNode(Node node, bool insertCopy = false)
        {
            var nodeTypeName = node.GetType().FullName;
            var nodeToInsert = insertCopy ? _nodeProvider.CreateNodeFromName(nodeTypeName) : node;
            nodeToInsert.Visible = false;
            nodeToInsert.Model.X = _diagram.GetDiagramPointFromViewPointX(X);
            nodeToInsert.Model.Y = _diagram.GetDiagramPointFromViewPointX(Y);
            _diagram.AddNodeInteractively(nodeToInsert);
        }

        private void ProviderNodeRegistered(Node node)
        {
            var result = new SearchResult() { Node = node, DisplayName = node.Name };
            AddAndInitializeNewNode(result);
        }

        private void FocusTextBox(TextBox textBox)
        {
            if (textBox != null)
            {
                textBox.Focus();
                Keyboard.Focus(textBox);
            }
        }

        private void PropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SearchPhrase))
            {
                UpdateFilterdList();
            }
        }

        private void UpdateFilterdList()
        {
            FilteredNodesList.Clear();
            _rootNode.GetMatches(SearchPhrase.Trim().ToLower()).Take(MaxSearchResults).ForEach(FilteredNodesList.Add);
            FilteredNodesList.ForEach(n => n.IsSelected = false);
            SelectedNodeIndex = -1;
            SelectedNodeIndex = 0;
        }
    }

    [AddINotifyPropertyChangedInterface]
    public class SearchResult
    {
        public string DisplayName { get; set; }

        public string SearchPhrase => DisplayName.ToLower();

        public Node Node { get; set; }

        public bool IsSelected { get; set; }
    }
}