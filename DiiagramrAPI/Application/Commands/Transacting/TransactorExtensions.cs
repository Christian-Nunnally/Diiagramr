namespace DiiagramrAPI.Application.Commands.Transacting
{
    public static class TransactorExtensions
    {
        public static void Transact(this ITransactor transactor, IReversableCommand command, IReversableCommand undoCommand, object parameter)
        {
            var commandWithCustomUndo = new CustomUndoCommand(command, undoCommand);
            transactor.Transact(commandWithCustomUndo, parameter);
        }

        public static void Transact(this ITransactor transactor, IReversableCommand command)
        {
            transactor.Transact(command, parameter: null);
        }
    }
}