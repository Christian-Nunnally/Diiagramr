using DiiagramrAPI.Application.Commands.Transacting;
using System;
using System.Windows.Input;

namespace DiiagramrAPI.Application.ShellCommands.EditCommands
{
    internal class RedoToolbarCommand : ShellCommandBase, IToolbarCommand, IHotkeyCommand
    {
        private readonly ITransactor _transactor;

        public RedoToolbarCommand(Func<ITransactor> transactor)
        {
            _transactor = transactor();
        }

        public string ParentName => "Edit";

        public float Weight => .05f;

        public override string Name => "Redo";

        public Key Hotkey => Key.Y;

        public bool RequiresCtrlModifierKey => true;

        public bool RequiresAltModifierKey => false;

        public bool RequiresShiftModifierKey => false;

        protected override bool CanExecuteInternal()
        {
            return _transactor.CanRedo();
        }

        protected override void ExecuteInternal(object parameter)
        {
            _transactor.Redo();
        }
    }
}