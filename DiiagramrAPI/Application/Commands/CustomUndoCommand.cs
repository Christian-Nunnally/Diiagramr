using System;

namespace DiiagramrAPI.Application.Commands
{
    /// <summary>
    /// A command that you can wrap around another command to override its undo action.
    /// </summary>
    public class CustomUndoCommand : IReversableCommand
    {
        private readonly IReversableCommand _doCommand;
        private readonly IReversableCommand _undoCommand;

        public CustomUndoCommand(IReversableCommand doCommand, IReversableCommand undoCommand)
        {
            _doCommand = doCommand;
            _undoCommand = undoCommand;
        }

        public Action Execute(object parameter)
        {
            _doCommand.Execute(parameter);
            return () => _undoCommand.Execute(parameter);
        }
    }
}