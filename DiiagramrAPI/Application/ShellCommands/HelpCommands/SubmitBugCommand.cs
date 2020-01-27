﻿namespace DiiagramrAPI.Application.ShellCommands.HelpCommands
{
    public class SubmitBugCommand : ToolBarCommand
    {
        public override string Name => "Submit Bug";

        public override string Parent => "Help";

        public override float Weight => .0f;

        public static void GoToSite(string url)
        {
            System.Diagnostics.Process.Start("cmd", $"/C start {url}");
        }

        protected override void ExecuteInternal(object parameter)
        {
            GoToSite("https://github.com/Christian-Nunnally/visual-drop/issues");
        }
    }
}