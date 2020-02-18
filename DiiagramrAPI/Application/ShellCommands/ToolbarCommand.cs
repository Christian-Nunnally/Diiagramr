using System.Windows.Input;

namespace DiiagramrAPI.Application.ShellCommands
{
    public abstract class ToolBarCommand : ShellCommandBase
    {
        public virtual Key Hotkey { get; } = Key.None;

        public virtual bool RequiresCtrlModifierKey { get; }

        public virtual bool RequiresAltModifierKey { get; }

        public virtual bool RequiresShiftModifierKey { get; }

        protected override bool CanExecuteInternal()
        {
            return true;
        }
    }
}