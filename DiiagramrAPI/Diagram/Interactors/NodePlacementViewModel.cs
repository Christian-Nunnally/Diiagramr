using DiiagramrAPI.PluginNodeApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DiiagramrAPI.Diagram.Interactors
{
    public class NodePlacementViewModel : DiagramInteractor
    {
        public PluginNode InsertingNodeViewModel { get; set; }

        public override void ProcessInteraction(DiagramInteractionEventArguments interaction)
        {
            if (interaction.Type == InteractionType.MouseMoved)
            {
                MoveInsertingNode(interaction.Diagram, interaction.MousePosition);
            }
        }

        private void MoveInsertingNode(DiagramViewModel diagram, Point mouseLocation)
        {
            if (InsertingNodeViewModel != null)
            {
                InsertingNodeViewModel.X = diagram.GetPointRelativeToPanAndZoomX(mouseLocation.X) - InsertingNodeViewModel.Width / 2.0 - DiagramViewModel.NodeBorderWidth;
                InsertingNodeViewModel.Y = diagram.GetPointRelativeToPanAndZoomY(mouseLocation.Y) - InsertingNodeViewModel.Height / 2.0 - DiagramViewModel.NodeBorderWidth;
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
            InsertingNodeViewModel = interaction.Diagram.NodeViewModels.LastOrDefault();
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
        }
    }
}
