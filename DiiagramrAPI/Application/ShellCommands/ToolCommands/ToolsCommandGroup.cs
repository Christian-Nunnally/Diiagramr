namespace DiiagramrAPI.Application.ShellCommands.ToolCommands
{
    public class ToolsCommandGroup : ShellCommandBase, IToolbarCommand
    {
        public override string Name => "Tools";

        public float Weight => 0.5f;

        public string ParentName => null;

        protected override bool CanExecuteInternal()
        {
            return true;
        }

        protected override void ExecuteInternal(object parameter)
        {
        }
    }
}