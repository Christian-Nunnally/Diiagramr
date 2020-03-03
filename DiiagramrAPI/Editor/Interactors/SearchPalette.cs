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
    public class SearchPalette : DiagramInteractor
    {
        private readonly INodeProvider _nodeProvider;
        private readonly FastTextSearchNode<SearchResult> _rootNode = new FastTextSearchNode<SearchResult>();
        private Diagram _diagram;
        private bool _shouldStopinteraction = false;
        private Node _nodeToInsert;
        private int _selectedNodeIndex = -1;

        public SearchPalette(Func<INodeProvider> nodeProvider)
        {
            Weight = 0;
            _nodeProvider = nodeProvider();
            if (_nodeProvider is NodeProvider provider)
            {
                provider.NodeRegistered += ProviderNodeRegistered;
            }
            PropertyChanged += PropertyChangedHandler;
            AddResultsToSearchTree();
        }

        public double PreviewNodePositionX { get; set; }
        public double PreviewNodePositionY { get; set; }
        public double PreviewNodeScaleX { get; set; }
        public double PreviewNodeScaleY { get; set; }
        public string SearchPhrase { get; set; } = string.Empty;
        public BindableCollection<SearchResult> FilteredNodesList { get; } = new BindableCollection<SearchResult>();

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
                    PreviewNode(FilteredNodesList[_selectedNodeIndex].Node);
                }
                else
                {
                    NodeToPreview = null;
                }
            }
        }

        public Node NodeToPreview { get; set; }

        public override void ProcessInteraction(DiagramInteractionEventArguments interaction)
        {
        }

        public override bool ShouldStartInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.KeyDown
                && interaction.Key == Key.Space
                && !interaction.IsModifierKeyPressed;
        }

        public override bool ShouldStopInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.RightMouseDown
                || interaction.Type == InteractionType.LeftMouseDown
                || _shouldStopinteraction;
        }

        public void SearchTextBoxLostFocus()
        {
        }

        public void SearchTextBoxLoaded(object sender, RoutedEventArgs e)
        {
            FocusTextBox(sender as TextBox);
        }

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

        public override void StartInteraction(DiagramInteractionEventArguments interaction)
        {
            _shouldStopinteraction = false;
            _diagram = interaction.Diagram;
            X = interaction.MousePosition.X;
            Y = interaction.MousePosition.Y;
            SearchPhrase = string.Empty;
            SelectedNodeIndex = -1;
        }

        public override void StopInteraction(DiagramInteractionEventArguments interaction)
        {
            if (_nodeToInsert != null)
            {
                BeginInsertingNode(_nodeToInsert, true);
            }
            _shouldStopinteraction = false;
            _nodeToInsert = null;
        }

        public void NodeMouseEnterHandler(object sender, MouseEventArgs e)
        {
            if (!(((Border)sender).DataContext is SearchResult searchResult))
            {
                return;
            }

            FilteredNodesList.ForEach(n => n.IsSelected = false);
            searchResult.IsSelected = true;
            PreviewNode(searchResult.Node);
        }

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
            nodeToInsert.NodeModel.X = _diagram.GetDiagramPointFromViewPointX(X);
            nodeToInsert.NodeModel.Y = _diagram.GetDiagramPointFromViewPointX(Y);
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
            _rootNode.GetMatches(SearchPhrase.Trim().ToLower()).ForEach(FilteredNodesList.Add);
            FilteredNodesList.ForEach(n => n.IsSelected = false);
            SelectedNodeIndex = -1;
            SelectedNodeIndex = 0;
        }

        private void PreviewNode(Node node)
        {
            NodeToPreview = node;
            const int workingWidth = 100;
            const int workingHeight = 100;

            var totalNodeWidth = NodeToPreview.Width + Diagram.NodeBorderWidth * 2.0;
            var totalNodeHeight = NodeToPreview.Height + Diagram.NodeBorderWidth * 2.0;
            PreviewNodeScaleX = workingWidth / totalNodeWidth;
            PreviewNodeScaleY = workingHeight / totalNodeHeight;

            PreviewNodeScaleX = Math.Min(PreviewNodeScaleX, PreviewNodeScaleY);
            PreviewNodeScaleY = Math.Min(PreviewNodeScaleX, PreviewNodeScaleY);

            var newWidth = totalNodeWidth * PreviewNodeScaleX + 1;
            var newHeight = totalNodeHeight * PreviewNodeScaleY + 1;

            PreviewNodePositionX = (workingWidth - newWidth) / 2.0;
            PreviewNodePositionY = (workingHeight - newHeight) / 2.0;
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