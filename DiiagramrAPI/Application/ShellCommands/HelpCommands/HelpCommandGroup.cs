namespace DiiagramrAPI.Application.ShellCommands.HelpCommands
{
    public class HelpCommandGroup : TopLevelToolBarCommand
    {
        public override string Name => "Help";

        public override float Weight => 0.6f;
    }
}