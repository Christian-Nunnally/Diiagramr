namespace DiiagramrAPI.Application.Commands.Transacting
{
    /// <summary>
    /// Represents an object that can undo and redo <see cref="IReversableCommand"/>s.
    /// </summary>
    public interface ITransactor
    {
        /// <summary>
        /// Transact this command. This can do different additional work depending on the implementation but ultimately this executes the command.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="parameter">The parameter to pass to the command.</param>
        void Transact(IReversableCommand command, object parameter);

        /// <summary>
        /// Undoes the last command that has been executed. Should only be called if the last command was recently executed.
        /// </summary>
        void Undo();

        /// <summary>
        /// Redos the last undone command. Should only be called if the last command undone was undone recently.
        /// </summary>
        void Redo();

        /// <summary>
        /// Whether an action can be undone.
        /// </summary>
        /// <returns>True if <see cref="Undo"/> would actaully undo an item.</returns>
        bool CanUndo();

        /// <summary>
        /// Whether an action can be redone.
        /// </summary>
        /// <returns>True if <see cref="Redo"/> would actaully redo an item.</returns>
        bool CanRedo();
    }
}