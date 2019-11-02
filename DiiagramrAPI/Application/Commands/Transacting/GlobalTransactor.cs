using System.Collections.Generic;

namespace DiiagramrAPI.Application.Commands.Transacting
{
    public class GlobalTransactor : Transactor
    {
        protected override Stack<UndoRedo> RedoStack => _globalRedoStack;
        protected override Stack<UndoRedo> UndoStack => _globalUndoStack;
        private static Stack<UndoRedo> _globalRedoStack { get; } = new Stack<UndoRedo>();
        private static Stack<UndoRedo> _globalUndoStack { get; } = new Stack<UndoRedo>();
    }
}