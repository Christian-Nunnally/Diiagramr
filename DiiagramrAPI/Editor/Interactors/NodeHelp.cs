using DiiagramrAPI.Editor.Diagrams;
using System.Linq;

namespace DiiagramrAPI.Editor.Interactors
{
    public class NodeHelp : DiagramInteractor
    {
        private bool _shouldStopInteraction = false;

        public string NodeName { get; set; }
        public string NodeHelpText { get; set; }

        public override void ProcessInteraction(DiagramInteractionEventArguments interaction)
        {
            if (interaction.Type == InteractionType.KeyUp)
            {
                _shouldStopInteraction = true;
            }
        }

        public override bool ShouldStartInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.KeyDown
                && interaction.ViewModelUnderMouse is Node
                && interaction.Key == System.Windows.Input.Key.H;
        }

        public override bool ShouldStopInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type != InteractionType.MouseMoved
                && interaction.Type != InteractionType.KeyUp
                && _shouldStopInteraction;
        }

        public override void StartInteraction(DiagramInteractionEventArguments interaction)
        {
            X = (interaction.Diagram.ViewWidth / 2) - 250;
            Y = (interaction.Diagram.ViewHeight / 2) - 200;
            _shouldStopInteraction = false;
            var node = (Node)interaction.ViewModelUnderMouse;
            NodeName = node.Name;
            if (node.GetType().GetCustomAttributes(typeof(HelpAttribute), true).FirstOrDefault() is HelpAttribute help)
            {
                NodeHelpText = help.HelpText;
            }
        }

        public override void StopInteraction(DiagramInteractionEventArguments interaction)
        {
        }
    }
}