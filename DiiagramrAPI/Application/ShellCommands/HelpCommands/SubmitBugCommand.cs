namespace DiiagramrAPI.Application.ShellCommands.HelpCommands
{
    public class SubmitBugCommand : ShellCommandBase, IToolbarCommand
    {
        public override string Name => "Submit Bug";

        public float Weight => .0f;

        public string ParentName => "Help";

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
            GoToSite("https://github.com/Christian-Nunnally/visual-drop/issues");
        }
    }
}