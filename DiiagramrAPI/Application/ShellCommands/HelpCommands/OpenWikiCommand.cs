namespace DiiagramrAPI.Application.ShellCommands.HelpCommands
{
    public class OpenWikiCommand : ShellCommandBase, IToolbarCommand
    {
        public override string Name => "Open Wiki";

        public string ParentName => "Help";

        public float Weight => 0.3f;

        public static void GoToSite(string url)
        {
            System.Diagnostics.Process.Start("cmd", $"/C start {url}");
        }

        protected override bool CanExecuteInternal()
        {
            return true;
        }

        protected override void ExecuteInternal(object parameter)
        {
            GoToSite("https://github.com/Christian-Nunnally/visual-drop/wiki");
        }
    }
}