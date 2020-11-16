using System;

namespace DiiagramrAPI.Application.Commands
{
    /// <summary>
    /// A command that you can wrap around another command to override its undo action.
    /// </summary>
    public class CustomUndoCommand : IReversableCommand
    {
        private readonly IReversableCommand _command;
        private readonly IReversableCommand _undoCommand;

        /// <summary>
        /// Creates a new instance of <see cref="CustomUndoCommand"/>.
        /// </summary>
        /// <param name="command">The command to execute during do.</param>
        /// <param name="undoCommand">The command to execute to undo the original <paramref name="command"/></param>
        public CustomUndoCommand(IReversableCommand command, IReversableCommand undoCommand)
        {
            _command = command;
            _undoCommand = undoCommand;
        }

        /// <inheritdoc/>
        public Action Execute(object parameter)
        {
            _command.Execute(parameter);
            return () => _undoCommand.Execute(parameter);
        }
    }
}