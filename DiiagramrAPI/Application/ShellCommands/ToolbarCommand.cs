namespace DiiagramrAPI.Application.ShellCommands
{
    public abstract class ToolBarCommand : ShellCommandBase
    {
        protected override bool CanExecuteInternal()
        {
            return true;
        }
    }
}