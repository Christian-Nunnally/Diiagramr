namespace DiiagramrAPI.Application.Commands.Transacting
{
    /// <summary>
    /// Helpful <see cref="ITransactor"/> extension methods for simplifying command execution.
    /// </summary>
    public static class TransactorExtensions
    {
        /// <summary>
        /// Execute a command with a custom undo command. Useful for complex undo scenarios.
        /// </summary>
        /// <param name="transactor">The transactor to execute the command with.</param>
        /// <param name="command">The command to execute.</param>
        /// <param name="undoCommand">The command that will undo the <paramref name="command"/>.</param>
        /// <param name="parameter">The parameter to pass into the command.</param>
        public static void Transact(this ITransactor transactor, IReversableCommand command, IReversableCommand undoCommand, object parameter)
        {
            var commandWithCustomUndo = new CustomUndoCommand(command, undoCommand);
            transactor.Transact(commandWithCustomUndo, parameter);
        }

        /// <summary>
        /// Executes a command without providing a parameter.
        /// </summary>
        /// <param name="transactor">The transactor to execute the command with.</param>
        /// <param name="command">The command to execute with a null parameter.</param>
        public static void Transact(this ITransactor transactor, IReversableCommand command)
        {
            transactor.Transact(command, parameter: null);
        }
    }
}