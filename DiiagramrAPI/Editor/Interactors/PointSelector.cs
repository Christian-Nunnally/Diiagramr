using DiiagramrAPI.Editor.Diagrams;
using System.Windows;

namespace DiiagramrAPI.Editor.Interactors
{
    public class PointSelector : DiagramInteractor
    {
        private Point _mouseDownPoint;

        public PointSelector()
        {
            Weight = 1;
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

            if (interaction.ViewModelUnderMouse is Node pluginNode)
            {
                if (!pluginNode.IsSelected)
                {
                    if (!interaction.IsCtrlKeyPressed)
                    {
                        interaction.Diagram.UnselectNodes();
                    }

                    interaction.Diagram.UnselectTerminals();
                    pluginNode.IsSelected = true;
                }
            }
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

        private void ProcessMouseDownInteraction(DiagramInteractionEventArguments interaction)
        {
            _mouseDownPoint = interaction.MousePosition;
        }

        private void ProcessMouseUpInteraction(DiagramInteractionEventArguments interaction)
        {
            if (_mouseDownPoint.Equals(interaction.MousePosition))
            {
                if (interaction.ViewModelUnderMouse is Diagram)
                {
                    interaction.Diagram.UnselectNodes();
                    interaction.Diagram.UnselectTerminals();
                }
                else if (interaction.ViewModelUnderMouse is Node pluginNode)
                {
                    if (!pluginNode.IsSelected)
                    {
                        if (!interaction.IsCtrlKeyPressed)
                        {
                            interaction.Diagram.UnselectNodes();
                        }

                        interaction.Diagram.UnselectTerminals();
                        pluginNode.IsSelected = true;
                    }
                }
            }
        }
    }
}