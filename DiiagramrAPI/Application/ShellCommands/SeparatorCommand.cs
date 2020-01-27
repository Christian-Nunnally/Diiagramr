namespace DiiagramrAPI.Application.ShellCommands
{
    public abstract class SeparatorCommand : ToolBarCommand
    {
        public override string Name => string.Empty;

        protected override void ExecuteInternal(object parameter)
        {
        }
    }
}