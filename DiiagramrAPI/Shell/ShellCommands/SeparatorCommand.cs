namespace DiiagramrAPI.Shell.ShellCommands
{
    public class FileSeparatorCommand1 : SeparatorCommand
    {
        public override string Parent => "Project";
        public override float Weight => 0.6f;
    }

    public class FileSeparatorCommand2 : SeparatorCommand
    {
        public override string Parent => "Project";
        public override float Weight => 0.3f;
    }

    public class HelpSeparatorCommand1 : SeparatorCommand
    {
        public override string Parent => "Help";
        public override float Weight => 0.5f;
    }

    public abstract class SeparatorCommand : ToolBarCommand
    {
        public override string Name => "";

        internal override void ExecuteInternal(IShell shell, object parameter)
        {
        }
    }
}