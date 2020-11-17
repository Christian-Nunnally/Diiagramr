namespace DiiagramrAPI.Editor.Interactors
{
    /// <summary>
    /// Allows the user to close a diagram by pressing control + the left arrow.
    /// </summary>
    public class DiagramCloser : DiagramInteractor
    {
        /// <inheritdoc/>
        public override void ProcessInteraction(DiagramInteractionEventArguments interaction)
        {
            interaction.Diagram.RequestClose();
        }

        /// <inheritdoc/>
        public override bool ShouldStartInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.KeyDown
                && !interaction.IsAltKeyPressed
                && !interaction.IsShiftKeyPressed
                && interaction.IsCtrlKeyPressed
                && interaction.Key == System.Windows.Input.Key.Left;
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