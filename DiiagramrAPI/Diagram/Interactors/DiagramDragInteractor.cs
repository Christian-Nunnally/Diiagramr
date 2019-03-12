using System.Windows;

namespace DiiagramrAPI.Diagram.Interactors
{
    public class DiagramDragInteractor : DiagramInteractor
    {
        public Point StartMouseLocation { get; set; }
        public double StartPanX { get; private set; }
        public double StartPanY { get; private set; }

        public override void ProcessInteraction(DiagramInteractionEventArguments interaction)
        {
            if (interaction.Type == InteractionType.MouseMoved)
            {
                var diagram = interaction.Diagram;
                var deltaX = interaction.MousePosition.X - StartMouseLocation.X;
                var deltaY = interaction.MousePosition.Y - StartMouseLocation.Y;
                diagram.PanX = StartPanX + deltaX;
                diagram.PanY = StartPanY + deltaY;
                diagram.PanNotify();
            }
        }

        public override bool ShouldStartInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.LeftMouseDown && interaction.ViewModelMouseIsOver is DiagramViewModel;
        }

        public override bool ShouldStopInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.LeftMouseUp;
        }

        public override void StartInteraction(DiagramInteractionEventArguments interaction)
        {
            StartMouseLocation = interaction.MousePosition;
            StartPanX = interaction.Diagram.PanX;
            StartPanY = interaction.Diagram.PanY;
        }

        public override void StopInteraction(DiagramInteractionEventArguments interaction)
        {
        }
    }
}
