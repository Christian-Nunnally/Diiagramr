using DiiagramrAPI.Editor.Diagrams;

namespace DiiagramrAPI.Editor.Interactors
{
    /// <summary>
    /// Allows the user to scroll in and out of the diagram using the mouse wheel.
    /// </summary>
    public class DiagramZoomer : DiagramInteractor
    {
        private const double MaximumZoom = 3.0;
        private const double MinimumZoom = 0.4;
        private const double ZoomAmount = .1;

        /// <inheritdoc/>
        public override void ProcessInteraction(DiagramInteractionEventArguments interaction)
        {
            var diagram = interaction.Diagram;
            var mousePosition = interaction.MousePosition;
            var diagramStart = diagram.GetDiagramPointFromViewPoint(mousePosition);

            var zoom = interaction.MouseWheelDelta > 0 ? 1.0 + ZoomAmount : 1.0 - ZoomAmount;
            var newZoom = diagram.Zoom * zoom;
            SetZoom(diagram, newZoom);

            var diagramEnd = diagram.GetDiagramPointFromViewPoint(mousePosition);

            diagram.PanX -= diagramStart.X - diagramEnd.X;
            diagram.PanY -= diagramStart.Y - diagramEnd.Y;
        }

        /// <inheritdoc/>
        public override bool ShouldStartInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.MouseWheel;
        }

        /// <inheritdoc/>
        public override bool ShouldStopInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.MouseWheel;
        }

        /// <inheritdoc/>
        public override void StartInteraction(DiagramInteractionEventArguments interaction)
        {
        }

        /// <inheritdoc/>
        public override void StopInteraction(DiagramInteractionEventArguments interaction)
        {
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
    }
}