using DiiagramrAPI.PluginNodeApi;
using System.Windows;

namespace DiiagramrAPI.Diagram.Interactors
{
    public class PointSelectorViewModel : DiagramInteractor
    {
        private Point _mouseDownPoint;

        public PointSelectorViewModel()
        {
            Weight = 1;
        }

        public override bool ShouldStartInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.LeftMouseDown
                || interaction.Type == InteractionType.LeftMouseUp;
        }

        public override bool ShouldStopInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.LeftMouseDown
                || interaction.Type == InteractionType.LeftMouseUp;
        }

        public override void StartInteraction(DiagramInteractionEventArguments interaction)
        {
        }

        public override void StopInteraction(DiagramInteractionEventArguments interaction)
        {
        }

        public override void ProcessInteraction(DiagramInteractionEventArguments interaction)
        {
            if (interaction.Type == InteractionType.LeftMouseDown)
            {
                ProcessMouseDownInteraction(interaction);
            }
            else if (interaction.Type == InteractionType.LeftMouseUp)
            {
                ProcessMouseUpInteraction(interaction);
            }

            if (interaction.ViewModelMouseIsOver is PluginNode pluginNode)
            {
                if (!pluginNode.IsSelected)
                {
                    if (!interaction.IsCtrlKeyPressed)
                    {
                        interaction.Diagram.UnselectNodes();
                    }
                    pluginNode.IsSelected = true;
                }
            }
        }

        private void ProcessMouseUpInteraction(DiagramInteractionEventArguments interaction)
        {
            if (_mouseDownPoint.Equals(interaction.MousePosition))
            {
                if (interaction.ViewModelMouseIsOver is DiagramViewModel)
                {
                    interaction.Diagram.UnselectNodes();
                    interaction.Diagram.UnselectTerminals();
                }
                else if (interaction.ViewModelMouseIsOver is PluginNode pluginNode)
                {
                    if (!pluginNode.IsSelected)
                    {
                        if (!interaction.IsCtrlKeyPressed)
                        {
                            interaction.Diagram.UnselectNodes();
                        }
                        pluginNode.IsSelected = true;
                    }
                }
            }
        }

        private void ProcessMouseDownInteraction(DiagramInteractionEventArguments interaction)
        {
            _mouseDownPoint = interaction.MousePosition;
        }
    }
}