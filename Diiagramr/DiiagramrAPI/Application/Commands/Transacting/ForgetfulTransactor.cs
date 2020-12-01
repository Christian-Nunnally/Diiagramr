using System;

namespace DiiagramrAPI.Application.Commands.Transacting
{
    /// <summary>
    /// A transactor that simply executes commands without remebering them. Does not provide undo/redo functionality.
    /// </summary>
    public class ForgetfulTransactor : ITransactor
    {
        private ForgetfulTransactor()
        {
        }

        public event Action Transacted;

        /// <summary>
        /// Gets the instance of the <see cref="ForgetfulTransactor"/>. There only needs to be one instance because the <see cref="ForgetfulTransactor"/> has no state.
        /// </summary>
        public static ForgetfulTransactor Instance { get; } = new ForgetfulTransactor();

        /// <inheritdoc/>
        public void Transact(IReversableCommand command, object parameter)
        {
            command.Execute(parameter);
            Transacted?.Invoke();
        }

        /// <inheritdoc/>
        public void Undo() { }

        /// <inheritdoc/>
        public void Redo() { }

        /// <inheritdoc/>
        public bool CanUndo() => false;

        /// <inheritdoc/>
        public bool CanRedo() => false;
    }
}