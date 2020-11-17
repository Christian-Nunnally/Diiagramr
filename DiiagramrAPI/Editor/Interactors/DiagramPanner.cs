using DiiagramrAPI.Editor.Diagrams;
using System;
using System.Windows;

namespace DiiagramrAPI.Editor.Interactors
{
    /// <summary>
    /// Allows the user to pan the diagram by clicking and dragging on the diagram background.
    /// </summary>
    public class DiagramPanner : DiagramInteractor
    {
        private const double MinimumMouseDeltaToStartPanning = 5;
        private bool _reachedMinimunMouseDeltaToStartPanning;

        /// <summary>
        /// The location that the mouse was clicked down for the pan interaction.
        /// </summary>
        public Point StartMouseLocation { get; set; }

        /// <summary>
        /// Gets the amount the diagram was panned in the x diraction prior to this pan interaction
        /// </summary>
        public double StartPanX { get; private set; }

        /// <summary>
        /// Gets the amount the diagram was panned in the y diraction prior to this pan interaction
        /// </summary>
        public double StartPanY { get; private set; }

        /// <inheritdoc/>
        public override void ProcessInteraction(DiagramInteractionEventArguments interaction)
        {
            var diagram = interaction.Diagram;
            if (interaction.Type == InteractionType.MouseMoved)
            {
                var mousePosition = interaction.MousePosition;
                ProcessMouseMoved(diagram, mousePosition);
            }
            else if (interaction.Type == InteractionType.LeftMouseUp && !_reachedMinimunMouseDeltaToStartPanning)
            {
                ProcessMouseMouseUpAfterNotPanning(diagram);
            }
        }

        /// <inheritdoc/>
        public override bool ShouldStartInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.LeftMouseDown
                && interaction.ViewModelUnderMouse is Diagram
                && !interaction.IsModifierKeyPressed;
        }

        /// <inheritdoc/>
        public override bool ShouldStopInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.LeftMouseUp;
        }

        /// <inheritdoc/>
        public override void StartInteraction(DiagramInteractionEventArguments interaction)
        {
            _reachedMinimunMouseDeltaToStartPanning = false;
            StartMouseLocation = interaction.MousePosition;
            StartPanX = interaction.Diagram.PanX;
            StartPanY = interaction.Diagram.PanY;
        }

        /// <inheritdoc/>
        public override void StopInteraction(DiagramInteractionEventArguments interaction)
        {
        }

        private static void ProcessMouseMouseUpAfterNotPanning(Diagram diagram)
        {
            diagram.UnselectNodes();
            diagram.UnselectTerminals();
        }

        private void ProcessMouseMoved(Diagram diagram, Point mousePosition)
        {
            var deltaX = mousePosition.X - StartMouseLocation.X;
            var deltaY = mousePosition.Y - StartMouseLocation.Y;

            if (!_reachedMinimunMouseDeltaToStartPanning)
            {
                var distance = Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));
                _reachedMinimunMouseDeltaToStartPanning = distance > MinimumMouseDeltaToStartPanning;
            }

            if (_reachedMinimunMouseDeltaToStartPanning)
            {
                diagram.PanX = StartPanX + deltaX;
                diagram.PanY = StartPanY + deltaY;
            }
        }
    }
}