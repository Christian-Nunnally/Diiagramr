using DiiagramrAPI.Application.Commands;
using DiiagramrAPI.Application.Commands.Transacting;
using DiiagramrAPI.Commands;
using System;
using System.Linq;
using System.Windows.Input;

namespace DiiagramrAPI.Editor.Interactors
{
    /// <summary>
    /// Allows the user to deleted the selected nodes by pressing the delete key.
    /// </summary>
    public class NodeDeleter : DiagramInteractor
    {
        private readonly ITransactor _transactor;

        /// <summary>
        /// Creates a new instance of <see cref="NodeDeleter"/>.
        /// </summary>
        /// <param name="transactorFactory">A factory that returns a <see cref="ITransactor"/> to tractact the deletes through.</param>
        public NodeDeleter(Func<ITransactor> transactorFactory)
        {
            _transactor = transactorFactory.Invoke();
        }

        /// <inheritdoc/>
        public override void ProcessInteraction(DiagramInteractionEventArguments interaction)
        {
            var diagram = interaction.Diagram;
            var selectedNodes = diagram.Nodes.Where(n => n.IsSelected).ToArray();
            var unwireAndRemoveAllNodesCommand = new MapCommand(new UnwireAndDeleteNodeCommand(diagram));
            _transactor.Transact(unwireAndRemoveAllNodesCommand, selectedNodes);

            if (!diagram.Nodes.Any())
            {
                diagram.ResetPanAndZoom();
            }
        }

        /// <inheritdoc/>
        public override bool ShouldStartInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.KeyDown && interaction.Key == Key.Delete;
        }

        /// <inheritdoc/>
        public override bool ShouldStopInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.KeyDown && interaction.Key == Key.Delete;
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