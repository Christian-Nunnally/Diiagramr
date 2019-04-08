﻿using DiiagramrAPI.Shell;

namespace DiiagramrAPI.Service.Commands.ToolCommands
{
    public class OpenWikiCommand : ToolBarCommand
    {
        public override string Name => "Open Wiki";
        public override string Parent => "Help";
        public override float Weight => 1.0f;

        internal override void ExecuteInternal(IShell shell, object parameter)
        {
            GoToSite("https://github.com/Christian-Nunnally/visual-drop/wiki");
        }

        public static void GoToSite(string url)
        {
            System.Diagnostics.Process.Start(url);
        }
    }
}
