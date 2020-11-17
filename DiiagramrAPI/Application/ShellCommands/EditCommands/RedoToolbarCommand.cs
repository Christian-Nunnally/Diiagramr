using DiiagramrAPI.Application.Commands.Transacting;
using System;
using System.Windows.Input;

namespace DiiagramrAPI.Application.ShellCommands.EditCommands
{
    /// <summary>
    /// A toolbar command that redoes the last undone command.
    /// </summary>
    internal class RedoToolbarCommand : ShellCommandBase, IToolbarCommand, IHotkeyCommand
    {
        private readonly ITransactor _transactor;

        /// <summary>
        /// Creates a new instance of <see cref="RedoToolbarCommand"/>.
        /// </summary>
        /// <param name="transactor">The transactor to redo using.</param>
        public RedoToolbarCommand(Func<ITransactor> transactor)
        {
            _transactor = transactor();
        }

        /// <inheritdoc/>
        public string ParentName => "Edit";

        /// <inheritdoc/>
        public float Weight => .05f;

        /// <inheritdoc/>
        public override string Name => "Redo";

        /// <inheritdoc/>
        public Key Hotkey => Key.Y;

        /// <inheritdoc/>
        public bool RequiresCtrlModifierKey => true;

        /// <inheritdoc/>
        public bool RequiresAltModifierKey => false;

        /// <inheritdoc/>
        public bool RequiresShiftModifierKey => false;

        /// <inheritdoc/>
        protected override bool CanExecuteInternal()
        {
            return _transactor.CanRedo();
        }

        /// <inheritdoc/>
        protected override void ExecuteInternal(object parameter)
        {
            _transactor.Redo();
        }
    }
}