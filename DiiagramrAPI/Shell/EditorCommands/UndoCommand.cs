using System;

namespace DiiagramrAPI.Shell.EditorCommands
{
    /// <summary>
    /// Wraps a command so that, when transacted, it becomes an undo action on the stack. This command, once undone, can not be redone.
    /// </summary>
    // TODO: turn in to an extension method
    public class UndoCommand : ICommand
    {
        public UndoCommand(ICommand command)
        {
            _command = command;
        }

        private bool _hasBeenUndone;
        private readonly ICommand _command;

        public Action Execute(object parameter) => Undo(parameter);

        private Action Undo(object parameter) => () =>
        {
            if (!_hasBeenUndone)
            {
                _command.Execute(parameter);
                _hasBeenUndone = true;
            }
        };
    }
}
