namespace DiiagramrAPI.Editor.Interactors
{
    public class HotkeyHelp : DiagramInteractor
    {
        private bool _shouldStopInteraction = false;

        public override void ProcessInteraction(DiagramInteractionEventArguments interaction)
        {
            if (interaction.Type == InteractionType.KeyUp)
            {
                _shouldStopInteraction = true;
            }
        }

        public override bool ShouldStartInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.KeyDown && interaction.Key == System.Windows.Input.Key.Oem2;
        }

        public override bool ShouldStopInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type != InteractionType.MouseMoved && _shouldStopInteraction;
        }

        public override void StartInteraction(DiagramInteractionEventArguments interaction)
        {
            X = (interaction.Diagram.ViewWidth / 2) - 250;
            Y = (interaction.Diagram.ViewHeight / 2) - 300;
            _shouldStopInteraction = false;
        }

        public override void StopInteraction(DiagramInteractionEventArguments interaction)
        {
        }
    }
}