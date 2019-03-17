namespace DiiagramrAPI.Diagram.Interactors
{
    public class MouseEnterLeaveInteractor : DiagramInteractor
    {
        private IMouseEnterLeaveReaction ReactionMouseIsCurrentlyIn { get; set; }

        public MouseEnterLeaveInteractor()
        {
            Weight = 1.0;
        }

        public override void ProcessInteraction(DiagramInteractionEventArguments interaction)
        {
        }

        public override bool ShouldStartInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.MouseMoved;
        }

        public override bool ShouldStopInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.MouseMoved;
        }

        public override void StartInteraction(DiagramInteractionEventArguments interaction)
        {
            if (interaction.ViewModelMouseIsOver is IMouseEnterLeaveReaction reaction)
            {
                if (reaction != ReactionMouseIsCurrentlyIn)
                {
                    ReactionMouseIsCurrentlyIn?.MouseLeft();
                    ReactionMouseIsCurrentlyIn = reaction;
                    ReactionMouseIsCurrentlyIn.MouseEntered();
                }
            }
        }

        public override void StopInteraction(DiagramInteractionEventArguments interaction)
        {
        }
    }
}
