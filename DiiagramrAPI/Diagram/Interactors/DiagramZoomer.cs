namespace DiiagramrAPI.Diagram.Interactors
{
    public class DiagramZoomer : DiagramInteractor
    {
        private const double MinimumZoom = 0.4;
        private const double MaximumZoom = 3.0;
        private const double ZoomAmount = .1;

        public override void ProcessInteraction(DiagramInteractionEventArguments interaction)
        {
            var diagram = interaction.Diagram;
            var mousePosition = interaction.MousePosition;
            var diagramStartX = diagram.GetDiagramPointFromViewPointX(mousePosition.X);
            var diagramStartY = diagram.GetDiagramPointFromViewPointY(mousePosition.Y);

            var zoom = interaction.MouseWheelDelta > 0 ? 1.0 + ZoomAmount : 1.0 - ZoomAmount;
            var newZoom = diagram.Zoom * zoom;
            SetZoom(diagram, newZoom);

            var diagramEndX = diagram.GetDiagramPointFromViewPointX(mousePosition.X);
            var diagramEndY = diagram.GetDiagramPointFromViewPointY(mousePosition.Y);

            diagram.PanX -= diagramStartX - diagramEndX;
            diagram.PanY -= diagramStartY - diagramEndY;
        }

        private void SetZoom(Diagram diagram, double zoom)
        {
            if (zoom < MinimumZoom)
            {
                diagram.Zoom = MinimumZoom;
                return;
            }
            else if (zoom > MaximumZoom)
            {
                diagram.Zoom = MaximumZoom;
                return;
            }
            diagram.Zoom = zoom;
        }

        public override bool ShouldStartInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.MouseWheel;
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
