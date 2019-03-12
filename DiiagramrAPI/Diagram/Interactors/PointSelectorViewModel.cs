using DiiagramrAPI.PluginNodeApi;
using System.Windows;

namespace DiiagramrAPI.Diagram.Interactors
{
    public class PointSelectorViewModel : DiagramInteractor
    {
        private Point _interactionStartPoint;
        private bool _cancelInteraction;

        public override bool ShouldStartInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.LeftMouseDown;
        }

        public override bool ShouldStopInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type != InteractionType.MouseMoved;
        }

        public override void StartInteraction(DiagramInteractionEventArguments interaction)
        {
            _interactionStartPoint = interaction.MousePosition;
            _cancelInteraction = false;
        }

        public override void StopInteraction(DiagramInteractionEventArguments interaction)
        {
            if (_cancelInteraction)
            {
                return;
            }
            if (!(interaction.ViewModelMouseIsOver is TerminalViewModel))
            {
                interaction.Diagram.UnselectTerminals();
            }
            if (interaction.ViewModelMouseIsOver is DiagramViewModel)
            {
                interaction.Diagram.UnselectNodes();
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

        public override void ProcessInteraction(DiagramInteractionEventArguments interaction)
        {
            if (interaction.Type == InteractionType.MouseMoved)
            {
                if (_interactionStartPoint.X - interaction.MousePosition.X > 5
                 || _interactionStartPoint.Y - interaction.MousePosition.Y > 5)
                {
                    _cancelInteraction = true;
                }
            }
        }
    }
}