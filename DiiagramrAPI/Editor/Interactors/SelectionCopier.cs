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
    /// <summary>
    /// Allows the user to copy all of the selected nodes by pressing ctrl+c.
    /// </summary>
    public class SelectionCopier : DiagramInteractor
    {
        private readonly INodeProvider _nodeProvider;
        private readonly ITransactor _transactor;
        private IEnumerable<NodeModel> _copiedNodes;

        /// <summary>
        /// Creates a new instance of <see cref="SelectionCopier"/>.
        /// </summary>
        /// <param name="transactorFactory">A factory that returns an instance of <see cref="ITransactor"/>.</param>
        /// <param name="nodeProviderFactory">A factory that returns an instance of <see cref="INodeProvider"/>.</param>
        public SelectionCopier(Func<ITransactor> transactorFactory, Func<INodeProvider> nodeProviderFactory)
        {
            _nodeProvider = nodeProviderFactory();
            _transactor = transactorFactory();
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public override bool ShouldStartInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.KeyDown
                && !interaction.IsAltKeyPressed
                && !interaction.IsShiftKeyPressed
                && interaction.IsCtrlKeyPressed
                && (interaction.Key == Key.C || interaction.Key == Key.V);
        }

        /// <inheritdoc/>
        public override bool ShouldStopInteraction(DiagramInteractionEventArguments interaction)
        {
            return true;
        }

        /// <inheritdoc/>
        public override void StartInteraction(DiagramInteractionEventArguments interaction)
        {
        }

        /// <inheritdoc/>
        public override void StopInteraction(DiagramInteractionEventArguments interaction)
        {
        }
    }
}