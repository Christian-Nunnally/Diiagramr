using DiiagramrAPI.Application.Commands.Transacting;
using System.Collections.Generic;

namespace DiiagramrAPI.Application.Commands
{
    /// <summary>
    /// A command with free undo functionality as long as the only actions <see cref="Execute(ITransactor)"/> does are other commands.
    /// </summary>
    public abstract class BasicComposingCommand : TransactingCommand
    {
        private readonly IList<IReversableCommand> _composedCommands = new List<IReversableCommand>();

        /// <summary>
        /// Adds a command to the list of commands that <see cref="Execute(ITransactor, object)"/> will execute.
        /// </summary>
        /// <param name="command">The command to add.</param>
        public void ComposeCommand(IReversableCommand command)
        {
            _composedCommands.Add(command);
        }

        /// <inheritdoc/>
        protected override void Execute(ITransactor transactor, object parameter)
        {
            foreach (var composedCommand in _composedCommands)
            {
                transactor.Transact(composedCommand, parameter);
            }
        }
    }
}