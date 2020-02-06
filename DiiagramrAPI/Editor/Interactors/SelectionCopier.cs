using DiiagramrAPI.Service.Editor;
using DiiagramrModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace DiiagramrAPI.Editor.Interactors
{
    public class SelectionCopier : DiagramInteractor
    {
        private IEnumerable<NodeModel> _copiedNodes;
        private INodeProvider _nodeProvider;

        public SelectionCopier(Func<INodeProvider> nodeProviderFactory)
        {
            _nodeProvider = nodeProviderFactory();
        }

        public override void ProcessInteraction(DiagramInteractionEventArguments interaction)
        {
            if (interaction.Key == Key.C)
            {
                var selectedNodes = interaction.Diagram.Nodes.Where(n => n.IsSelected);
                _copiedNodes = selectedNodes.Select(s => s.Model.Copy()).OfType<NodeModel>().ToList();
            }
            else
            {
                foreach (var copiedNode in _copiedNodes)
                {
                    var node = _nodeProvider.CreateNodeFromModel((NodeModel)copiedNode.Copy());
                    node.IsSelected = true;
                    var diagramMousePoint = interaction.Diagram.GetDiagramPointFromViewPoint(interaction.MousePosition);
                    node.X = diagramMousePoint.X;
                    node.Y = diagramMousePoint.Y;
                    interaction.Diagram.AddNode(node);
                }
            }
        }

        public override bool ShouldStartInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.KeyDown
                && !interaction.IsAltKeyPressed
                && !interaction.IsShiftKeyPressed
                && interaction.IsCtrlKeyPressed
                && (interaction.Key == Key.C || interaction.Key == Key.V);
        }

        public override bool ShouldStopInteraction(DiagramInteractionEventArguments interaction)
        {
            return true;
        }

        public override void StartInteraction(DiagramInteractionEventArguments interaction)
        {
        }

        public override void StopInteraction(DiagramInteractionEventArguments interaction)
        {
        }
    }
}