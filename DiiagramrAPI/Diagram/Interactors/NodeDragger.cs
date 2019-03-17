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
                foreach (var otherNode in interaction.Diagram.NodeViewModels.Where(n => n.IsSelected))
                {
                    otherNode.X += deltaX / interaction.Diagram.Zoom;
                    otherNode.Y += deltaY / interaction.Diagram.Zoom;
                }
                PreviousMouseLocation = interaction.MousePosition;
                interaction.Diagram.UpdateDiagramBoundingBox();
                interaction.Diagram.ShowSnapGrid = true;
                Mouse.SetCursor(Cursors.SizeAll);
            }
        }

        public override bool ShouldStartInteraction(DiagramInteractionEventArguments interaction)
        {
            if ((interaction.Type == InteractionType.LeftMouseDown
                || interaction.Type == InteractionType.MouseMoved)
                && interaction.ViewModelMouseIsOver is Node node
                && !interaction.IsCtrlKeyPressed)
            {
                var mouseX = interaction.MousePosition.X;
                var mouseY = interaction.MousePosition.Y;
                var diagram = interaction.Diagram;
                var nodeLeft = diagram.GetViewPointFromDiagramPointX(node.X + Diagram.NodeBorderWidth);
                var nodeTop = diagram.GetViewPointFromDiagramPointY(node.Y + Diagram.NodeBorderWidth);
                var nodeRight = diagram.GetViewPointFromDiagramPointX(node.X + node.Width + Diagram.NodeBorderWidth);
                var nodeBottom = diagram.GetViewPointFromDiagramPointY(node.Y + node.Height + Diagram.NodeBorderWidth);

                if (mouseX > nodeLeft && mouseX < nodeRight)
                {
                    if (mouseY > nodeTop && mouseY < nodeBottom)
                    {
                        return false;
                    }
                }
                Mouse.SetCursor(Cursors.SizeAll);
                return interaction.Type == InteractionType.LeftMouseDown;
            }
            return false;
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
