using DiiagramrAPI.Editor.Diagrams;
using System;
using System.Windows;

namespace DiiagramrAPI.Editor.Interactors
{
    public class DiagramPanner : DiagramInteractor
    {
        private const double MinimumMouseDeltaToStartPanning = 5;
        private bool _reachedMinimunMouseDeltaToStartPanning;
        public Point StartMouseLocation { get; set; }
        public double StartPanX { get; private set; }
        public double StartPanY { get; private set; }

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

        public override bool ShouldStartInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.LeftMouseDown
                && interaction.ViewModelUnderMouse is Diagram
                && !interaction.IsCtrlKeyPressed;
        }

        public override bool ShouldStopInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.LeftMouseUp;
        }

        public override void StartInteraction(DiagramInteractionEventArguments interaction)
        {
            _reachedMinimunMouseDeltaToStartPanning = false;
            StartMouseLocation = interaction.MousePosition;
            StartPanX = interaction.Diagram.PanX;
            StartPanY = interaction.Diagram.PanY;
        }

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