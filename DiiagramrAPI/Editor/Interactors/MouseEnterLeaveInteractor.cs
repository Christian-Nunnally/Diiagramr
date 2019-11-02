namespace DiiagramrAPI.Editor.Interactors
{
    public class MouseEnterLeaveInteractor : DiagramInteractor
    {
        public MouseEnterLeaveInteractor()
        {
            Weight = 1.0;
        }

        private IMouseEnterLeaveReaction ReactionMouseIsCurrentlyIn { get; set; }

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
            if (interaction.ViewModelUnderMouse is IMouseEnterLeaveReaction reaction)
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