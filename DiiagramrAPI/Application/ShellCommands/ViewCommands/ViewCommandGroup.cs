namespace DiiagramrAPI.Application.ShellCommands.ToolCommands
{
    public class ViewCommandGroup : ShellCommandBase, IToolbarCommand
    {
        public override string Name => "View";

        public float Weight => 0.4f;

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