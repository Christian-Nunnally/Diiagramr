﻿using System;
using System.Collections.Generic;

namespace DiiagramrAPI.Shell.Commands.Transacting
{
    /// <summary>
    /// A command with free undo functionality as long as the only actions <see cref="Execute(ITransactor)"/> does are other commands.
    /// </summary>
    public abstract class TransactingCommand : ICommand
    {
        public virtual Action Execute(object parameter)
        {
            Transactor transactor = new Transactor();
            Execute(transactor, parameter);
            return () => UndoRedo(transactor);
        }

        private void UndoRedo(Transactor transactor)
        {
            transactor.UndoAll();
            transactor.MoveRedoStackBackToUndo();
        }

        protected abstract void Execute(ITransactor transactor, object parameter);
    }
}
