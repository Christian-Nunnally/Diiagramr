using System;

namespace DiiagramrAPI.Application.Commands.Transacting
{
    /// <summary>
    /// A command with free undo functionality as long as the only actions <see cref="Execute(ITransactor)"/> does are other commands.
    /// </summary>
    public abstract class TransactingCommand : IReversableCommand
    {
        /// <inheritdoc/>
        public virtual Action Execute(object parameter)
        {
            Transactor transactor = new Transactor();
            Execute(transactor, parameter);
            return () => UndoRedo(transactor);
        }

        /// <summary>
        /// Do the action.
        /// </summary>
        /// <param name="transactor">Transactor to provide extensibility to the command infastructure.</param>
        /// <param name="parameter">General purpose command parameter.</param>
        /// <returns>An action that will undo what execute did.</returns>
        protected abstract void Execute(ITransactor transactor, object parameter);

        private void UndoRedo(Transactor transactor)
        {
            while (transactor.CanUndo()) transactor.Undo();
            transactor.MoveRedoStackBackToUndo();
        }
    }
}