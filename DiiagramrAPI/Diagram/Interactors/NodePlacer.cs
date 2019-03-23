using System.Linq;
using System.Windows;

namespace DiiagramrAPI.Diagram.Interactors
{
    public class NodePlacer : DiagramInteractor
    {
        public Node InsertingNodeViewModel { get; set; }

        public override void ProcessInteraction(DiagramInteractionEventArguments interaction)
        {
            if (interaction.Type == InteractionType.MouseMoved)
            {
                MoveInsertingNode(interaction.Diagram, interaction.MousePosition);
                interaction.Diagram.UpdateDiagramBoundingBox();
                InsertingNodeViewModel.Visible = true;
            }
        }

        private void MoveInsertingNode(Diagram diagram, Point mouseLocation)
        {
            if (InsertingNodeViewModel != null)
            {
                InsertingNodeViewModel.Model.X = diagram.GetDiagramPointFromViewPointX(mouseLocation.X) - InsertingNodeViewModel.Width / 2.0 - Diagram.NodeBorderWidth;
                InsertingNodeViewModel.Model.Y = diagram.GetDiagramPointFromViewPointY(mouseLocation.Y) - InsertingNodeViewModel.Height / 2.0 - Diagram.NodeBorderWidth;
                InsertingNodeViewModel.X = diagram.GetDiagramPointFromViewPointX(mouseLocation.X) - InsertingNodeViewModel.Width / 2.0 - Diagram.NodeBorderWidth;
                InsertingNodeViewModel.Y = diagram.GetDiagramPointFromViewPointY(mouseLocation.Y) - InsertingNodeViewModel.Height / 2.0 - Diagram.NodeBorderWidth;
            }
        }

        public override bool ShouldStartInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.NodeInserted; 
        }

        public override bool ShouldStopInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.LeftMouseDown || interaction.Type == InteractionType.RightMouseDown;
        }

        public override void StartInteraction(DiagramInteractionEventArguments interaction)
        {
            InsertingNodeViewModel = interaction.Diagram.Nodes.LastOrDefault();
            interaction.Diagram.ShowSnapGrid = true;
        }

        public override void StopInteraction(DiagramInteractionEventArguments interaction)
        {
            if (interaction.Type == InteractionType.RightMouseDown)
            {
                interaction.Diagram.RemoveNode(InsertingNodeViewModel);
            }
            else
            {
                InsertingNodeViewModel.X = interaction.Diagram.SnapToGrid(InsertingNodeViewModel.X);
                InsertingNodeViewModel.Y = interaction.Diagram.SnapToGrid(InsertingNodeViewModel.Y);
            }
            InsertingNodeViewModel = null;
            interaction.Diagram.ShowSnapGrid = false;
        }
    }
}
