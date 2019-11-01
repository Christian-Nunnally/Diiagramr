using System;
using System.Collections.Generic;

namespace DiiagramrAPI.Shell.Commands.Transacting
{
    public class Transactor : ITransactor
    {
        protected virtual Stack<UndoRedo> UndoStack { get; } = new Stack<UndoRedo>();
        protected virtual Stack<UndoRedo> RedoStack { get; } = new Stack<UndoRedo>();

        public void Transact(ICommand command, object parameter)
        {
            RedoStack.Clear();
            Action undo = command.Execute(parameter);
            Action redo = () => command.Execute(parameter);
            UndoStack.Push(new UndoRedo(undo, redo));
        }

        public void Redo()
        {
            if (RedoStack.Count > 0)
            {
                var undoRedo = RedoStack.Pop();
                undoRedo.Redo();
                UndoStack.Push(undoRedo);
            }
        }

        public void Undo()
        {
            if (UndoStack.Count > 0)
            {
                var undoRedo = UndoStack.Pop();
                undoRedo.Undo();
                RedoStack.Push(undoRedo);
            }
        }

        public void UndoAll()
        {
            while (UndoStack.Count > 0)
            {
                Undo();
            }
        }

        public void MoveRedoStackBackToUndo()
        {
            while (RedoStack.Count > 0)
            {
                UndoStack.Push(RedoStack.Pop());
            }
        }

        protected struct UndoRedo
        {
            public readonly Action Undo;
            public readonly Action Redo;

            public UndoRedo(Action undo, Action redo)
            {
                Undo = undo;
                Redo = redo;
            }
        }
    }
}
