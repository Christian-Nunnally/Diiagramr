﻿using System;

namespace DiiagramrAPI.Application.Commands
{
    /// <summary>
    /// Wraps a command so that, when transacted, it becomes an undo action on the stack. This command, once undone, can not be redone.
    /// </summary>
    public class UndoCommand : IReversableCommand
    {
        private readonly IReversableCommand _command;

        private bool _hasBeenUndone;

        public UndoCommand(IReversableCommand command)
        {
            _command = command;
        }

        public Action Execute(object parameter) => Undo(parameter);

        private Action Undo(object parameter) => () =>
        {
            if (!_hasBeenUndone)
            {
                _command.Execute(parameter);
                _hasBeenUndone = true;
            }
        };
    }
}