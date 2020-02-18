namespace DiiagramrAPI.Application.ShellCommands.DiagnosticsCommands
{
    public class DiagnosticsCommandGroup : ShellCommandBase, IToolbarCommand
    {
        public override string Name => "Diagnostics";

        public float Weight => 1.0f;

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