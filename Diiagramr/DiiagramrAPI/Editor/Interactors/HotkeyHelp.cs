namespace DiiagramrAPI.Editor.Interactors
{
    /// <summary>
    /// Shows information about basic hotkeys to get started while the Oem2 ('/') is held.
    /// </summary>
    public class HotkeyHelp : DiagramInteractor
    {
        private bool _shouldStopInteraction = false;

        /// <inheritdoc/>
        public override void ProcessInteraction(DiagramInteractionEventArguments interaction)
        {
            if (interaction.Type == InteractionType.KeyUp)
            {
                _shouldStopInteraction = true;
            }
        }

        /// <inheritdoc/>
        public override bool ShouldStartInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.KeyDown && interaction.Key == System.Windows.Input.Key.Oem2;
        }

        /// <inheritdoc/>
        public override bool ShouldStopInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type != InteractionType.MouseMoved && _shouldStopInteraction;
        }

        /// <inheritdoc/>
        public override void StartInteraction(DiagramInteractionEventArguments interaction)
        {
            X = (interaction.Diagram.ViewWidth / 2) - 250;
            Y = (interaction.Diagram.ViewHeight / 2) - 300;
            _shouldStopInteraction = false;
        }

        /// <inheritdoc/>
        public override void StopInteraction(DiagramInteractionEventArguments interaction)
        {
        }
    }
}