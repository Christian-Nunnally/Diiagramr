using System.Collections.Generic;

namespace DiiagramrAPI.Application.Commands.Transacting
{
    public class GlobalTransactor : Transactor
    {
        protected override Stack<UndoRedo> RedoStack => GlobalRedoStack;

        protected override Stack<UndoRedo> UndoStack => GlobalUndoStack;

        private static Stack<UndoRedo> GlobalRedoStack { get; } = new Stack<UndoRedo>();

        private static Stack<UndoRedo> GlobalUndoStack { get; } = new Stack<UndoRedo>();
    }
}