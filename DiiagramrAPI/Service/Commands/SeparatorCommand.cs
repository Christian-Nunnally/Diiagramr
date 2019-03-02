﻿using DiiagramrAPI.Shell;
using DiiagramrAPI.ViewModel;

namespace DiiagramrAPI.Service.Commands
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

    public abstract class SeparatorCommand : ToolBarCommand
    {
        public override string Name => "";

        internal override void ExecuteInternal(IShell shell, object parameter)
        {
        }
    }
}
