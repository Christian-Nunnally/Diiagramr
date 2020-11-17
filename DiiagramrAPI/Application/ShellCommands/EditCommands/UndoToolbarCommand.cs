using DiiagramrAPI.Application.Commands.Transacting;
using System;
using System.Windows.Input;

namespace DiiagramrAPI.Application.ShellCommands.EditCommands
{
    /// <summary>
    /// A toolbar command that undoes the last done or redone command.
    /// </summary>
    internal class UndoToolbarCommand : ShellCommandBase, IToolbarCommand, IHotkeyCommand
    {
        private readonly ITransactor _transactor;

        /// <summary>
        /// Creates a new instance of <see cref="UndoToolbarCommand"/>.
        /// </summary>
        /// <param name="transactor">The transactor to undo using.</param>
        public UndoToolbarCommand(Func<ITransactor> transactor)
        {
            _transactor = transactor();
        }

        /// <inheritdoc/>
        public string ParentName => "Edit";

        /// <inheritdoc/>
        public float Weight => .05f;

        /// <inheritdoc/>
        public override string Name => "Undo";

        /// <inheritdoc/>
        public Key Hotkey => Key.Z;

        /// <inheritdoc/>
        public bool RequiresCtrlModifierKey => true;

        /// <inheritdoc/>
        public bool RequiresAltModifierKey => false;

        /// <inheritdoc/>
        public bool RequiresShiftModifierKey => false;

        /// <inheritdoc/>
        protected override bool CanExecuteInternal()
        {
            return _transactor.CanUndo();
        }

        /// <inheritdoc/>
        protected override void ExecuteInternal(object parameter)
        {
            _transactor.Undo();
        }
    }
}