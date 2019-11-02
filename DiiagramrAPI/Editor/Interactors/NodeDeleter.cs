using DiiagramrAPI.Commands;
using DiiagramrAPI.Shell.Commands;
using DiiagramrAPI.Shell.Commands.Transacting;
using System;
using System.Linq;
using System.Windows.Input;

namespace DiiagramrAPI.Editor.Interactors
{
    public class NodeDeleter : DiagramInteractor
    {
        private readonly ITransactor _transactor;

        public NodeDeleter(Func<ITransactor> _transactorFactory)
        {
            _transactor = _transactorFactory.Invoke();
        }

        public override void ProcessInteraction(DiagramInteractionEventArguments interaction)
        {
            var diagram = interaction.Diagram;
            var selectedNodes = diagram.Nodes.Where(n => n.IsSelected).ToArray();
            var unwireAndRemoveAllNodesCommand = new MapCommand(new UnwireAndDeleteNodeCommand(diagram));
            _transactor.Transact(unwireAndRemoveAllNodesCommand, selectedNodes);

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
