using DiiagramrAPI.Shell;

namespace DiiagramrAPI.Service.Commands.ToolCommands
{
    public class SubmitBugCommand : ToolBarCommand
    {
        public override string Name => "Submit Bug";
        public override string Parent => "Help";
        public override float Weight => .0f;

        internal override void ExecuteInternal(IShell shell, object parameter)
        {
            GoToSite("https://github.com/Christian-Nunnally/visual-drop/issues");
        }

        public static void GoToSite(string url)
        {
            System.Diagnostics.Process.Start(url);
        }
    }
}
