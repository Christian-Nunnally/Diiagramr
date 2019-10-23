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
            foreach (var node in selectedNodes)
            {
                var connectedWires = node.Terminals.SelectMany(t => t.Model.ConnectedWires);
                foreach (var wire in connectedWires)
                {
                    diagram.RemoveWire(wire);
                }
                diagram.RemoveNode(node);
            }

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
