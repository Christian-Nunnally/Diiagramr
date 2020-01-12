using System;
using System.Collections.Generic;

namespace DiiagramrAPI.Application.Commands.Transacting
{
    public class Transactor : ITransactor
    {
        protected virtual Stack<UndoRedo> RedoStack { get; } = new Stack<UndoRedo>();

        protected virtual Stack<UndoRedo> UndoStack { get; } = new Stack<UndoRedo>();

        public void MoveRedoStackBackToUndo()
        {
            while (RedoStack.Count > 0)
            {
                UndoStack.Push(RedoStack.Pop());
            }
        }

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

        public void Transact(ICommand command, object parameter)
        {
            RedoStack.Clear();
            Action undo = command.Execute(parameter);
            Func<Action> redo = () => command.Execute(parameter);
            UndoStack.Push(new UndoRedo(undo, redo));
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

        protected struct UndoRedo
        {
            public UndoRedo(Action undo, Func<Action> redo)
            {
                Undo = undo;
                Redo = redo;
            }

            public Func<Action> Redo { get; }

            public Action Undo { get; set; }
        }
    }
}