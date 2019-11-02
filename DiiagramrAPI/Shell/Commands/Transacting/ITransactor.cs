namespace DiiagramrAPI.Shell.Commands.Transacting
{
    public interface ITransactor
    {
        /// <summary>
        /// Redos the last undone command. Should only be called if the last command undone was undone recently.
        /// </summary>
        void Redo();

        /// <summary>
        /// Transact this command. This can do different additional work depending on the implementation but ultimately this executes the command.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="parameter">The parameter to pass to the command.</param>
        void Transact(ICommand command, object parameter);

        /// <summary>
        /// Undoes the last command that has been executed. Should only be called if the last command was recently executed.
        /// </summary>
        void Undo();
    }
}