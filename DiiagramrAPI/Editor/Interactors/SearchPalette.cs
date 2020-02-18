using DiiagramrAPI.Editor.Diagrams;
using DiiagramrAPI.Service.Editor;
using DiiagramrCore;
using PropertyChanged;
using Stylet;
using System;
using System.Collections.Generic;
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
        private Diagram _diagram;
        private bool _shouldStopinteraction = false;
        private int _selectedNodeIndex = -1;

        public SearchPalette(Func<INodeProvider> nodeProvider)
        {
            _nodeProvider = nodeProvider();
            AllSearchResults = _nodeProvider
                .GetRegisteredNodes()
                .Select(n => new SearchResult() { Node = n, DisplayName = n.Name })
                .ToList();
            if (_nodeProvider is NodeProvider provider)
            {
                provider.NodeRegistered += ProviderNodeRegistered;
            }
            PropertyChanged += PropertyChangedHandler;
            UpdateFilterdList();
        }

        public string SearchPhrase { get; set; } = string.Empty;

        public IList<SearchResult> AllSearchResults { get; set; }

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
                }
            }
        }

        public void BeginInsertingNode(Node node, bool insertCopy = false)
        {
            var nodeTypeName = node.GetType().FullName;
            var nodeToInsert = insertCopy ? _nodeProvider.CreateNodeFromName(nodeTypeName) : node;
            nodeToInsert.Visible = false;
            nodeToInsert.NodeModel.X = _diagram.GetDiagramPointFromViewPointX(X);
            nodeToInsert.NodeModel.Y = _diagram.GetDiagramPointFromViewPointX(Y);
            _diagram.AddNodeInteractively(nodeToInsert);
        }

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
                || interaction.Type == InteractionType.NodeInserted
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
                    BeginInsertingNode(node.Node, true);
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
        }

        public override void StopInteraction(DiagramInteractionEventArguments interaction)
        {
            _diagram = null;
        }

        private void ProviderNodeRegistered(Node node)
        {
            AllSearchResults.Add(new SearchResult() { Node = node, DisplayName = node.Name });
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
            AllSearchResults.Where(n => n.DisplayName.Contains(SearchPhrase)).ForEach(FilteredNodesList.Add);
        }

        [AddINotifyPropertyChangedInterface]
        public class SearchResult
        {
            public string DisplayName { get; set; }

            public Node Node { get; set; }

            public bool IsSelected { get; set; }
        }
    }
}