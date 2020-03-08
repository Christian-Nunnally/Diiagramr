using DiiagramrAPI.Application;
using DiiagramrAPI.Application.Dialogs;
using DiiagramrAPI.Editor.Diagrams;
using DiiagramrAPI.Service.Editor;
using System;

namespace DiiagramrAPI.Editor.Interactors
{
    public class NodeHelp : DiagramInteractor
    {
        private readonly INodeProvider _nodeProvider;
        private readonly DialogHostBase _dialogHost;
        private HelpDialog _helpDialog;

        public NodeHelp(
            Func<INodeProvider> nodeProviderFactory,
            Func<DialogHostBase> dialogHostFactory)
        {
            _nodeProvider = nodeProviderFactory();
            _dialogHost = dialogHostFactory();
        }

        public Node Node { get; set; }

        public string VisibleHelpText { get; set; }

        public override void ProcessInteraction(DiagramInteractionEventArguments interaction)
        {
        }

        public override bool ShouldStartInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.KeyDown
                && interaction.ViewModelUnderMouse is Node
                && interaction.Key == System.Windows.Input.Key.H;
        }

        public override bool ShouldStopInteraction(DiagramInteractionEventArguments interaction)
        {
            return true;
        }

        public override void StartInteraction(DiagramInteractionEventArguments interaction)
        {
            X = (interaction.Diagram.ViewWidth / 2) - 250;
            Y = (interaction.Diagram.ViewHeight / 2) - 200;
            var node = (Node)interaction.ViewModelUnderMouse;
            var nodeCopy = _nodeProvider.CreateNodeFromName(node.GetType().FullName);
            node.SetAdorner(null);
            _helpDialog = new HelpDialog(nodeCopy);
            _dialogHost.OpenDialog(_helpDialog);
        }

        public override void StopInteraction(DiagramInteractionEventArguments interaction)
        {
        }
    }
}