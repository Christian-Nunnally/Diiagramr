using DiiagramrAPI.Editor.Diagrams;

namespace DiiagramrAPI.Application.ShellCommands.ToolCommands
{
    public class EnableDataPropagationVisualsCommand : ShellCommandBase, IToolbarCommand
    {
        public EnableDataPropagationVisualsCommand()
        {
        }

        public override string Name => "Propagation Visuals";

        public string ParentName => "View";

        public float Weight => .2f;

        protected override bool CanExecuteInternal()
        {
            return true;
        }

        protected override void ExecuteInternal(object parameter)
        {
            Wire.ShowDataPropagation = !Wire.ShowDataPropagation;
        }
    }
}