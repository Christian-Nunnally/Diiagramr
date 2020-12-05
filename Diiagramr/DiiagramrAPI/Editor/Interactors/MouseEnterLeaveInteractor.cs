namespace DiiagramrAPI.Editor.Interactors
{
    /// <summary>
    /// Notifies view models that implement <see cref="IMouseEnterLeaveReaction"/> that the mouse has entered or left them.
    /// </summary>
    public class MouseEnterLeaveInteractor : DiagramInteractor
    {
        /// <summary>
        /// Creates a new instance of <see cref="MouseEnterLeaveInteractor"/>.
        /// </summary>
        public MouseEnterLeaveInteractor()
        {
            Weight = 1.0;
        }

        private IMouseEnterLeaveReaction ReactionMouseIsCurrentlyIn { get; set; }

        /// <inheritdoc/>
        public override void ProcessInteraction(DiagramInteractionEventArguments interaction)
        {
        }

        /// <inheritdoc/>
        public override bool ShouldStartInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.MouseMoved;
        }

        /// <inheritdoc/>
        public override bool ShouldStopInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.MouseMoved;
        }

        /// <inheritdoc/>
        public override void StartInteraction(DiagramInteractionEventArguments interaction)
        {
            if (interaction.ViewModelUnderMouse is IMouseEnterLeaveReaction reaction)
            {
                if (reaction != ReactionMouseIsCurrentlyIn)
                {
                    ReactionMouseIsCurrentlyIn?.MouseLeft();
                    ReactionMouseIsCurrentlyIn = reaction;
                    ReactionMouseIsCurrentlyIn.MouseEntered();
                }
            }
            else if (interaction.ViewModelUnderMouse is object)
            {
                ReactionMouseIsCurrentlyIn?.MouseLeft();
                ReactionMouseIsCurrentlyIn = null;
            }
        }

        /// <inheritdoc/>
        public override void StopInteraction(DiagramInteractionEventArguments interaction)
        {
        }
    }
}