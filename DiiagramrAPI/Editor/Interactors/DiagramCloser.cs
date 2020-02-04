namespace DiiagramrAPI.Editor.Interactors
{
    public class DiagramCloser : DiagramInteractor
    {
        public override void ProcessInteraction(DiagramInteractionEventArguments interaction)
        {
            interaction.Diagram.RequestClose();
        }

        public override bool ShouldStartInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.KeyDown
                && !interaction.IsAltKeyPressed
                && !interaction.IsShiftKeyPressed
                && interaction.IsCtrlKeyPressed
                && interaction.Key == System.Windows.Input.Key.Left;
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