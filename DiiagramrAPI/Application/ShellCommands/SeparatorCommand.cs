namespace DiiagramrAPI.Application.ShellCommands
{
    public abstract class SeparatorCommand : ToolBarCommand
    {
        public override string Name => string.Empty;

        internal override void ExecuteInternal(IApplicationShell shell, object parameter)
        {
        }
    }
}