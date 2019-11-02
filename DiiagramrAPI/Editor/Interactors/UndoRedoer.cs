using DiiagramrAPI.Shell.Commands.Transacting;
using System;
using System.Windows.Input;

namespace DiiagramrAPI.Editor.Interactors
{
    public class UndoRedoer : DiagramInteractor
    {
        private ITransactor _commandExecutor;

        public UndoRedoer(Func<ITransactor> commandExecutorFactory)
        {
            _commandExecutor = commandExecutorFactory.Invoke();
        }

        public override void ProcessInteraction(DiagramInteractionEventArguments interaction)
        {
        }

        public override bool ShouldStartInteraction(DiagramInteractionEventArguments interaction)
        {
            if (interaction.Type == InteractionType.KeyDown
                && interaction.IsCtrlKeyPressed)
            {
                if (interaction.Key == Key.Z)
                {
                    _commandExecutor.Undo();
                    return true;
                }
                else if (interaction.Key == Key.Y)
                {
                    _commandExecutor.Redo();
                    return true;
                }
            }
            return false;
        }

        public override bool ShouldStopInteraction(DiagramInteractionEventArguments interaction) => true;

        public override void StartInteraction(DiagramInteractionEventArguments interaction)
        {
        }

        public override void StopInteraction(DiagramInteractionEventArguments interaction)
        {
        }
    }
}