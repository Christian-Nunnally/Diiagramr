using DiiagramrAPI.Service;
using System.Linq;
using System.Windows.Input;

namespace DiiagramrAPI.Diagram.Interactors
{
    public class NodeDeleter : DiagramInteractor
    {
        public override void ProcessInteraction(DiagramInteractionEventArguments interaction)
        {
            var diagram = interaction.Diagram;
            var selectedNodes = diagram.Nodes.Where(n => n.IsSelected);
            selectedNodes.ForEach(diagram.RemoveNode);
            if (!diagram.Nodes.Any())
            {
                diagram.ResetPanAndZoom();
            }
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
