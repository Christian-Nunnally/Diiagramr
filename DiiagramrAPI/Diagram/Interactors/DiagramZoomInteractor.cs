namespace DiiagramrAPI.Diagram.Interactors
{
    public class DiagramZoomInteractor : DiagramInteractor
    {
        private const double MinimumZoom = 0.4;
        private const double MaximumZoom = 3.0;
        private const double NumberOfFrames = 3;
        private const double ZoomAmount = .2;

        public override void ProcessInteraction(DiagramInteractionEventArguments interaction)
        {
            var diagram = interaction.Diagram;
            var mousePosition = interaction.MousePosition;
            var abosuluteX = mousePosition.X * diagram.Zoom + diagram.PanX;
            var abosuluteY = mousePosition.Y * diagram.Zoom + diagram.PanY;

            var zoom = interaction.MouseWheelDelta > 0 ? ZoomAmount : -ZoomAmount;
            var newZoom = diagram.Zoom + zoom;
            if (newZoom < MinimumZoom)
            {
                diagram.Zoom = MinimumZoom;
                return;
            }
            else if (newZoom > MaximumZoom)
            {
                diagram.Zoom = MaximumZoom;
                return;
            }
            interaction.Diagram.Zoom += zoom;

            diagram.PanX = abosuluteX - mousePosition.X * diagram.Zoom;
            diagram.PanY = abosuluteY - mousePosition.Y * diagram.Zoom;
        }

        public override bool ShouldStartInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.MouseWheel && interaction.ViewModelMouseIsOver is DiagramViewModel;
        }

        public override bool ShouldStopInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.MouseWheel;
        }

        public override void StartInteraction(DiagramInteractionEventArguments interaction)
        {
        }

        public override void StopInteraction(DiagramInteractionEventArguments interaction)
        {
        }
    }
}
