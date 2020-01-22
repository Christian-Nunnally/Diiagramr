using DiiagramrAPI.Editor.Diagrams;

namespace DiiagramrAPI.Application.ShellCommands.ToolCommands
{
    public class EnableDataPropagationVisualsCommand : ToolBarCommand
    {
        public EnableDataPropagationVisualsCommand()
        {
        }

        public override string Name => "Propagation Visuals";

        public override string Parent => "View";

        public override float Weight => .5f;

        internal override void ExecuteInternal(IApplicationShell shell, object parameter)
        {
            Wire.ShowDataPropagation = !Wire.ShowDataPropagation;
        }
    }
}