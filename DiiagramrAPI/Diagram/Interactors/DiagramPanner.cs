using System;
using System.Windows;

namespace DiiagramrAPI.Diagram.Interactors
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
                var deltaX = interaction.MousePosition.X - StartMouseLocation.X;
                var deltaY = interaction.MousePosition.Y - StartMouseLocation.Y;

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
            else if (interaction.Type == InteractionType.LeftMouseUp && !_reachedMinimunMouseDeltaToStartPanning)
            {
                diagram.UnselectNodes();
                diagram.UnselectTerminals();
            }
        }

        public override bool ShouldStartInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.LeftMouseDown
                && interaction.ViewModelMouseIsOver is Diagram
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
    }
}
