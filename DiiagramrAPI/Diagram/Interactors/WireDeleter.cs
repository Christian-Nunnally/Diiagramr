namespace DiiagramrAPI.Diagram.Interactors
{
    public class WireDeleter : DiagramInteractor
    {
        public override void ProcessInteraction(DiagramInteractionEventArguments interaction)
        {
            if (interaction.ViewModelUnderMouse is Wire wire)
            {
                wire.DisconnectWire();
                interaction.Diagram.RemoveWire(wire);
            }
        }

        public override bool ShouldStartInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.LeftMouseDown
                && interaction.IsCtrlKeyPressed
                && interaction.ViewModelUnderMouse is Wire wire;
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
