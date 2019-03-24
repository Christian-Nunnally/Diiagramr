using System.Linq;
using System.Windows;

namespace DiiagramrAPI.Diagram.Interactors
{
    public class NodeDragger : DiagramInteractor
    {
        public Point PreviousMouseLocation { get; set; }

        public override void ProcessInteraction(DiagramInteractionEventArguments interaction)
        {
            if (interaction.Type == InteractionType.MouseMoved)
            {
                var diagram = interaction.Diagram;
                var mousePosition = interaction.MousePosition;
                ProcessMouseDown(diagram, mousePosition);
            }
        }

        private void ProcessMouseDown(Diagram diagram, Point mousePosition)
        {
            var deltaX = mousePosition.X - PreviousMouseLocation.X;
            var deltaY = mousePosition.Y - PreviousMouseLocation.Y;
            foreach (var otherNode in diagram.Nodes.Where(n => n.IsSelected))
            {
                otherNode.X += deltaX / diagram.Zoom;
                otherNode.Y += deltaY / diagram.Zoom;
            }
            PreviousMouseLocation = mousePosition;
            diagram.UpdateDiagramBoundingBox();
            diagram.ShowSnapGrid = true;
        }

        public override bool ShouldStartInteraction(DiagramInteractionEventArguments interaction)
        {
            if ((interaction.Type == InteractionType.LeftMouseDown)
                && interaction.ViewModelUnderMouse is Node node
                && !interaction.IsCtrlKeyPressed)
            {
                var mouseX = interaction.MousePosition.X;
                var mouseY = interaction.MousePosition.X;
                var diagram = interaction.Diagram;
                return IsMouseOverNodeBorder(node, mouseX, mouseY, diagram);
            }
            return false;
        }

        private static bool IsMouseOverNodeBorder(Node node, double mouseX, double mouseY, Diagram diagram)
        {
            var nodeLeft = diagram.GetViewPointFromDiagramPointX(node.X + Diagram.NodeBorderWidth);
            var nodeTop = diagram.GetViewPointFromDiagramPointY(node.Y + Diagram.NodeBorderWidth);
            var nodeRight = diagram.GetViewPointFromDiagramPointX(node.X + node.Width + Diagram.NodeBorderWidth);
            var nodeBottom = diagram.GetViewPointFromDiagramPointY(node.Y + node.Height + Diagram.NodeBorderWidth);
            return mouseX > nodeLeft
                || mouseX < nodeRight
                || mouseY > nodeTop
                || mouseY < nodeBottom;
        }

        public override bool ShouldStopInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.LeftMouseUp;
        }

        public override void StartInteraction(DiagramInteractionEventArguments interaction)
        {
            PreviousMouseLocation = interaction.MousePosition;
        }

        public override void StopInteraction(DiagramInteractionEventArguments interaction)
        {
            if (!interaction.IsCtrlKeyPressed)
            {
                foreach (var node in interaction.Diagram.Nodes.Where(n => n.IsSelected))
                {
                    node.X = interaction.Diagram.SnapToGrid(node.X);
                    node.Y = interaction.Diagram.SnapToGrid(node.Y);
                }
            }
            interaction.Diagram.ShowSnapGrid = false;
        }
    }
}
