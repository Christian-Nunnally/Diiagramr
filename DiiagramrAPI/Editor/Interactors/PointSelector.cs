using DiiagramrAPI.Editor.Diagrams;
using System.Windows;

namespace DiiagramrAPI.Editor.Interactors
{
    /// <summary>
    /// Allows the user to select things on a diagram by clicking on them.
    /// </summary>
    public class PointSelector : DiagramInteractor
    {
        private Point _mouseDownPoint;

        /// <summary>
        /// Creates a new instance of <see cref="PointSelector"/>
        /// </summary>
        public PointSelector()
        {
            Weight = 1;
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public override bool ShouldStartInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.LeftMouseDown
                || interaction.Type == InteractionType.LeftMouseUp;
        }

        /// <inheritdoc/>
        public override bool ShouldStopInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.LeftMouseDown
                || interaction.Type == InteractionType.LeftMouseUp;
        }

        /// <inheritdoc/>
        public override void StartInteraction(DiagramInteractionEventArguments interaction)
        {
        }

        /// <inheritdoc/>
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