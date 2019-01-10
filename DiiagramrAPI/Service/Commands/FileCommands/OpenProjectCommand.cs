﻿using DiiagramrAPI.ViewModel;

namespace DiiagramrAPI.Service.Commands.FileCommands
{
    public class OpenProjectCommand : DiiagramrCommand
    {
        public override string Parent => "File";
        public override string Name => "Open";
        public override float Weight => .9f;

        public override void Execute(ShellViewModel shell)
        {
            shell.ProjectManager.LoadProject();
        }
    }
}