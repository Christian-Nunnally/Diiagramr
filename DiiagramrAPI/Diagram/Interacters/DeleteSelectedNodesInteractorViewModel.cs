using DiiagramrAPI.Service;
using System;
using System.Linq;
using System.Windows.Input;

namespace DiiagramrAPI.Diagram.Interacters
{
    public class DeleteSelectedNodesInteractorViewModel : DiagramInteractor
    {
        public override void ProcessInteraction(DiagramInteractionEventArguments interaction)
        {
            var selectedNodes = interaction.Diagram.NodeViewModels.Where(n => n.IsSelected);
            selectedNodes.ForEach(interaction.Diagram.RemoveNode);
        }

        public override bool ShouldStartInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.KeyDown && interaction.Key == Key.Delete;
        }

        public override bool ShouldStopInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.KeyDown && interaction.Key == Key.Delete;
        }

        public override void StartInteraction(DiagramInteractionEventArguments interaction)
        {
        }

        public override void StopInteraction(DiagramInteractionEventArguments interaction)
        {
        }
    }
}
