using DiiagramrAPI.Application.Commands;
using DiiagramrAPI.Application.Commands.Transacting;
using DiiagramrAPI.Commands;
using DiiagramrAPI.Editor.Diagrams;
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
        private readonly INodeProvider _nodeProvider;
        private readonly ITransactor _transactor;
        private IEnumerable<NodeModel> _copiedNodes;

        public SelectionCopier(Func<ITransactor> transactorFactory, Func<INodeProvider> nodeProviderFactory)
        {
            _nodeProvider = nodeProviderFactory();
            _transactor = transactorFactory();
        }

        public override void ProcessInteraction(DiagramInteractionEventArguments interaction)
        {
            var selectedNodes = interaction.Diagram.Nodes.Where(n => n.IsSelected).ToList();
            selectedNodes.ForEach(n => n.IsSelected = false);
            if (interaction.Key == Key.C)
            {
                _copiedNodes = selectedNodes.Select(s => s.Model.Copy()).OfType<NodeModel>().ToList();
            }
            else
            {
                if (_copiedNodes == null)
                {
                    return;
                }
                var firstCopiedNodeX = _copiedNodes.FirstOrDefault()?.X ?? 0;
                var firstCopiedNodeY = _copiedNodes.FirstOrDefault()?.Y ?? 0;
                var nodesToInsertToDiagram = new List<Node>();
                foreach (var copiedNode in _copiedNodes)
                {
                    var node = _nodeProvider.CreateNodeFromModel((NodeModel)copiedNode.Copy());
                    node.IsSelected = true;
                    var diagramMousePoint = interaction.Diagram.GetDiagramPointFromViewPoint(interaction.MousePosition);
                    node.X -= firstCopiedNodeX - diagramMousePoint.X;
                    node.Y -= firstCopiedNodeY - diagramMousePoint.Y;
                    node.IsSelected = true;
                    nodesToInsertToDiagram.Add(node);
                    interaction.Diagram.AddNode(node);
                }
                _transactor.Transact(new UndoCommand(new MapCommand(new UnwireAndDeleteNodeCommand(interaction.Diagram))), nodesToInsertToDiagram);
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