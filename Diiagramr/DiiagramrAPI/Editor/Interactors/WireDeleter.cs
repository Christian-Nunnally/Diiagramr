using DiiagramrAPI.Application.Commands.Transacting;
using DiiagramrAPI.Commands;
using DiiagramrAPI.Editor.Diagrams;
using System;

namespace DiiagramrAPI.Editor.Interactors
{
    /// <summary>
    /// Allows the user to delete a wire by clicking on it.
    /// </summary>
    public class WireDeleter : DiagramInteractor
    {
        private readonly ITransactor _transactor;

        /// <summary>
        /// Creates a new instance of <see cref="WireDeleter"/>.
        /// </summary>
        /// <param name="transactorFactory">A factory that creates an <see cref="ITransactor"/> instance to transact the wire delete with.</param>
        public WireDeleter(Func<ITransactor> transactorFactory)
        {
            _transactor = transactorFactory.Invoke();
        }

        /// <inheritdoc/>
        public override void ProcessInteraction(DiagramInteractionEventArguments interaction)
        {
            if (interaction.ViewModelUnderMouse is Wire wire)
            {
                var deleteWireCommand = new DeleteWireCommand(interaction.Diagram);
                _transactor.Transact(deleteWireCommand, wire.WireModel);
            }
        }

        /// <inheritdoc/>
        public override bool ShouldStartInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.LeftMouseDown
                && interaction.ViewModelUnderMouse is Wire wire;
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