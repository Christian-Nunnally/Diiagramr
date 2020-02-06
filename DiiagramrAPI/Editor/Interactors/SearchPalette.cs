using DiiagramrAPI.Editor.Diagrams;
using DiiagramrAPI.Service.Editor;
using DiiagramrCore;
using Stylet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace DiiagramrAPI.Editor.Interactors
{
    public class SearchPalette : DiagramInteractor
    {
        private readonly INodeProvider _nodeProvider;
        private Diagram _diagram;

        public SearchPalette(Func<INodeProvider> nodeProvider)
        {
            _nodeProvider = nodeProvider();
            AvailableNodes = _nodeProvider.GetRegisteredNodes();
            PropertyChanged += PropertyChangedHandler;
            UpdateFilterdList();
        }

        public string SearchPhrase { get; set; } = string.Empty;

        public IEnumerable<Node> AvailableNodes { get; set; }

        public BindableCollection<Node> FilteredNodesList { get; } = new BindableCollection<Node>();

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
            return interaction.Type == InteractionType.RightMouseDown || interaction.Type == InteractionType.NodeInserted;
        }

        public override void StartInteraction(DiagramInteractionEventArguments interaction)
        {
            _diagram = interaction.Diagram;
            X = interaction.MousePosition.X;
            Y = interaction.MousePosition.Y;
        }

        public override void StopInteraction(DiagramInteractionEventArguments interaction)
        {
            _diagram = null;
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
            AvailableNodes.Where(n => n.Name.Contains(SearchPhrase)).ForEach(FilteredNodesList.Add);
        }
    }
}