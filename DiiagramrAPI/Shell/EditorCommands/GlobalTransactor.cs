using System.Collections.Generic;

namespace DiiagramrAPI.Shell.EditorCommands
{
    public class GlobalTransactor : Transactor
    {
        private static Stack<UndoRedo> _globalUndoStack { get; } = new Stack<UndoRedo>();
        private static Stack<UndoRedo> _globalRedoStack { get; } = new Stack<UndoRedo>();

        protected override Stack<UndoRedo> UndoStack => _globalUndoStack;
        protected override Stack<UndoRedo> RedoStack => _globalRedoStack;
    }
}
