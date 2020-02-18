using System.Windows.Input;

namespace DiiagramrAPI.Application.ShellCommands.FileCommands
{
    public class ProjectCommandGroup : ShellCommandBase, IToolbarCommand
    {
        public override string Name => "Project";

        public string ParentName => null;

        public float Weight => 0;

        protected override bool CanExecuteInternal()
        {
            return true;
        }

        protected override void ExecuteInternal(object parameter)
        {
        }
    }
}