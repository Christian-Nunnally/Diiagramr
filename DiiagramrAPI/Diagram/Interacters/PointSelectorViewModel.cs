using DiiagramrAPI.PluginNodeApi;

namespace DiiagramrAPI.Diagram.Interacters
{
    public class PointSelectorViewModel : DiagramInteractor
    {
        public override bool ShouldStartInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.LeftMouseDown;
        }

        public override bool ShouldStopInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.LeftMouseDown;
        }

        public override void StartInteraction(DiagramInteractionEventArguments interaction)
        {
        }

        public override void StopInteraction(DiagramInteractionEventArguments interaction)
        {
        }

        public override void ProcessInteraction(DiagramInteractionEventArguments interaction)
        {
            interaction.Diagram.UnselectTerminals();
            if (!interaction.IsCtrlKeyPressed)
            {
                interaction.Diagram.UnselectNodes();
            }
            if (interaction.ViewModelMouseIsOver is PluginNode pluginNode)
            {
                pluginNode.IsSelected = true;
            }
            interaction.Diagram.StopInteractor(this);
        }
    }
}