namespace DiiagramrAPI.Application.ShellCommands.HelpCommands
{
    public class HelpCommandGroup : ShellCommandBase, IToolbarCommand
    {
        public float Weight => 0.6f;

        public string ParentName => null;

        public override string Name => "Help";

        protected override bool CanExecuteInternal()
        {
            return true;
        }

        protected override void ExecuteInternal(object parameter)
        {
        }
    }
}