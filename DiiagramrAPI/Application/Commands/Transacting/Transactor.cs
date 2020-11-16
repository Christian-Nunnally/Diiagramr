using System;
using System.Collections.Generic;

namespace DiiagramrAPI.Application.Commands.Transacting
{
    /// <summary>
    /// Executes <see cref="IReversableCommand"/>s and allows for undoing the commands in the order applied.
    /// </summary>
    /// <remarks>
    /// It might be a good idea to a smarter implementation because some command's do not play nicely with this basic undo/redo.
    /// </remarks>
    public class Transactor : ITransactor
    {
        /// <summary>
        /// The stack of items that have been undone amd could be redone.
        /// </summary>
        protected virtual Stack<UndoRedo> RedoStack { get; } = new Stack<UndoRedo>();

        /// <summary>
        /// The stack of items that have been done and could be undone.
        /// </summary>
        protected virtual Stack<UndoRedo> UndoStack { get; } = new Stack<UndoRedo>();

        /// <inheritdoc/>
        public void Transact(IReversableCommand command, object parameter)
        {
            RedoStack.Clear();
            Action undo = command.Execute(parameter);
            Action redo() => command.Execute(parameter);
            UndoStack.Push(new UndoRedo(undo, redo));
        }

        /// <inheritdoc/>
        public void Undo()
        {
            if (UndoStack.Count > 0)
            {
                var undoRedo = UndoStack.Pop();
                undoRedo.Undo();
                RedoStack.Push(undoRedo);
            }
        }

        /// <inheritdoc/>
        public void Redo()
        {
            if (RedoStack.Count > 0)
            {
                var undoRedo = RedoStack.Pop();
                var newUndo = undoRedo.Redo();
                undoRedo.Undo = newUndo;
                UndoStack.Push(undoRedo);
            }
        }

        /// <inheritdoc/>
        public bool CanUndo() => UndoStack.Count > 0;

        /// <inheritdoc/>
        public bool CanRedo() => RedoStack.Count > 0;

        /// <summary>
        /// Moves the entire redo stack onto the undo stack, in order.
        /// </summary>
        public void MoveRedoStackBackToUndo()
        {
            while (RedoStack.Count > 0)
            {
                UndoStack.Push(RedoStack.Pop());
            }
        }

        /// <summary>
        /// Stuct used instead of a tuple to keep to code more organized.
        /// </summary>
        protected struct UndoRedo
        {
            internal UndoRedo(Action undo, Func<Action> redo)
            {
                Undo = undo;
                Redo = redo;
            }

            internal Func<Action> Redo { get; }

            internal Action Undo { get; set; }
        }
    }
}