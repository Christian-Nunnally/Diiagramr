using DiiagramrAPI.Application.Commands.Transacting;
using System.Collections.Generic;

namespace DiiagramrAPI.Application.Commands
{
    /// <summary>
    /// A command with free undo functionality as long as the only actions <see cref="Execute(ITransactor)"/> does are other commands.
    /// </summary>
    public abstract class BasicComposingCommand : TransactingCommand
    {
        private readonly IList<ICommand> _composedCommands = new List<ICommand>();

        public void ComposeCommand(ICommand command)
        {
            _composedCommands.Add(command);
        }

        protected override void Execute(ITransactor transactor, object parameter)
        {
            foreach (var composedCommand in _composedCommands)
            {
                transactor.Transact(composedCommand, parameter);
            }
        }
    }
}