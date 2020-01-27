namespace DiiagramrAPI.Application.ShellCommands
{
    // todo: maybe get rid of this interface, and instead let ToolbarViewModel decide how it wants to display them based on parent/child relationship of the commands.
    public abstract class TopLevelToolBarCommand : ToolBarCommand
    {
        protected override void ExecuteInternal(object parameter)
        {
        }
    }
}