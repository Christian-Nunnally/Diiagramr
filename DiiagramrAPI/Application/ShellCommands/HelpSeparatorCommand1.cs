namespace DiiagramrAPI.Application.ShellCommands
{

    public class HelpSeparatorCommand1 : SeparatorCommand
    {
        public override string Parent => "Help";

        public override float Weight => 0.5f;
    }
}