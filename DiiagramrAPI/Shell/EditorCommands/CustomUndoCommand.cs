using System;

namespace DiiagramrAPI.Shell.EditorCommands
{
    /// <summary>
    /// A command that you can wrap around another command to override its undo action.
    /// </summary>
    public class CustomUndoCommand : ICommand
    {
        private readonly ICommand _doCommand;
        private readonly ICommand _undoCommand;

        public CustomUndoCommand(ICommand doCommand, ICommand undoCommand)
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
