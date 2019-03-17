using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace DiiagramrAPI.Diagram.Interactors
{
    public class NodeDragger : DiagramInteractor
    {
        public Point PreviousMouseLocation { get; set; }

        public override void ProcessInteraction(DiagramInteractionEventArguments interaction)
        {
            if (interaction.Type == InteractionType.MouseMoved)
            {
                var deltaX = interaction.MousePosition.X - PreviousMouseLocation.X;
                var deltaY = interaction.MousePosition.Y - PreviousMouseLocation.Y;
                foreach (var node in interaction.Diagram.NodeViewModels.Where(n => n.IsSelected))
                {
                    node.X += deltaX / interaction.Diagram.Zoom;
                    node.Y += deltaY / interaction.Diagram.Zoom;
                }
                PreviousMouseLocation = interaction.MousePosition;
                interaction.Diagram.UpdateDiagramBoundingBox();
                interaction.Diagram.ShowSnapGrid = true;
            }
        }

        public override bool ShouldStartInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.LeftMouseDown 
                && interaction.ViewModelMouseIsOver is Node
                && !interaction.IsCtrlKeyPressed;
        }

        public override bool ShouldStopInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.LeftMouseUp;
        }

        public override void StartInteraction(DiagramInteractionEventArguments interaction)
        {
            Mouse.SetCursor(Cursors.SizeAll);
            PreviousMouseLocation = interaction.MousePosition;
        }

        public override void StopInteraction(DiagramInteractionEventArguments interaction)
        {
            if (!interaction.IsCtrlKeyPressed)
            {
                foreach (var node in interaction.Diagram.NodeViewModels.Where(n => n.IsSelected))
                {
                    node.X = interaction.Diagram.SnapToGrid(node.X);
                    node.Y = interaction.Diagram.SnapToGrid(node.Y);
                }
            }
            interaction.Diagram.ShowSnapGrid = false;
            Mouse.SetCursor(Cursors.Arrow);
        }
    }
}
