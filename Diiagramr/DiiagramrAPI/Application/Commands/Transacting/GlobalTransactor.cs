using System.Collections.Generic;

namespace DiiagramrAPI.Application.Commands.Transacting
{
    /// <summary>
    /// A static <see cref="Transactor"/> allowing process wide undo/redo.
    /// </summary>
    public class GlobalTransactor : Transactor
    {
        /// <inheritdoc/>
        protected override Stack<UndoRedo> UndoStack => GlobalUndoStack;

        /// <inheritdoc/>
        protected override Stack<UndoRedo> RedoStack => GlobalRedoStack;

        private static Stack<UndoRedo> GlobalUndoStack { get; } = new Stack<UndoRedo>();

        private static Stack<UndoRedo> GlobalRedoStack { get; } = new Stack<UndoRedo>();
    }
}