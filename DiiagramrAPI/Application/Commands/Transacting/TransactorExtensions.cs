namespace DiiagramrAPI.Application.Commands.Transacting
{
    public static class TransactorExtensions
    {
        public static void Transact(this ITransactor transactor, ICommand command, ICommand undoCommand, object parameter)
        {
            var commandWithCustomUndo = new CustomUndoCommand(command, undoCommand);
            transactor.Transact(commandWithCustomUndo, parameter);
        }
    }
}