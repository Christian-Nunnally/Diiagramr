using DiiagramrAPI.Application;
using DiiagramrAPI.Application.Dialogs;
using DiiagramrAPI.Editor.Diagrams;
using DiiagramrAPI.Service.Editor;
using System;

namespace DiiagramrAPI.Editor.Interactors
{
    /// <summary>
    /// Allows the user to press 'H' while thier mouse is over a node to open the help dialog for that node.
    /// </summary>
    public class NodeHelp : DiagramInteractor
    {
        private readonly INodeProvider _nodeProvider;
        private readonly DialogHostBase _dialogHost;
        private HelpDialog _helpDialog;

        /// <summary>
        /// Creates a new instance of <see cref="NodeHelp"/>.
        /// </summary>
        /// <param name="nodeProviderFactory">A factory that returns an instance of <see cref="INodeProvider"/>.</param>
        /// <param name="dialogHostFactory">A factory that returns an instance of <see cref="DialogHostBase"/>.</param>
        public NodeHelp(
            Func<INodeProvider> nodeProviderFactory,
            Func<DialogHostBase> dialogHostFactory)
        {
            _nodeProvider = nodeProviderFactory();
            _dialogHost = dialogHostFactory();
        }

        /// <inheritdoc/>
        public override void ProcessInteraction(DiagramInteractionEventArguments interaction)
        {
        }

        /// <inheritdoc/>
        public override bool ShouldStartInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.KeyDown
                && (interaction.ViewModelUnderMouse is Node || interaction.ViewModelUnderMouse is Terminal)
                && interaction.Key == System.Windows.Input.Key.H;
        }

        /// <inheritdoc/>
        public override bool ShouldStopInteraction(DiagramInteractionEventArguments interaction)
        {
            return true;
        }

        /// <inheritdoc/>
        public override void StartInteraction(DiagramInteractionEventArguments interaction)
        {
            var node = (Node)interaction.ViewModelUnderMouse;
            var nodeCopy = _nodeProvider.CreateNodeFromName(node.GetType().FullName);
            node.SetAdorner(null);
            _helpDialog = new HelpDialog(nodeCopy);
            _dialogHost.OpenDialog(_helpDialog);
        }

        /// <inheritdoc/>
        public override void StopInteraction(DiagramInteractionEventArguments interaction)
        {
        }
    }
}