using DiiagramrAPI.Application;
using System;

namespace DiiagramrAPI2.Application.Tools
{
    public class SaveFileWindow : ShellWindow
    {
        public override int MaxHeight => 100;

        public override int MaxWidth => 100;

        public override string Title => "Save File";

        public string InitialDirectory { get; internal set; }

        public string FileName { get; internal set; }

        public Action<string> SaveAction { get; internal set; }
    }
}