using DiiagramrAPI.Application;
using System;

namespace DiiagramrAPI2.Application.Dialogs
{
    public class LoadProjectDialog : Dialog
    {
        public override int MaxHeight => 300;

        public override int MaxWidth => 300;

        public override string Title { get; set; } = "Load Project";

        public string InitialDirectory { get; internal set; }

        public string FileName { get; internal set; }

        public Action<string> LoadAction { get; internal set; }

        public void LoadProject()
        {
            LoadAction(FileName);
        }
    }
}