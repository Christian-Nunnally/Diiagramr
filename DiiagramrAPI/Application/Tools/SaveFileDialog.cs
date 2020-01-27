using DiiagramrAPI.Application;
using System;

namespace DiiagramrAPI2.Application.Tools
{
    public class SaveFileDialog : Dialog
    {
        public override int MaxHeight => 300;

        public override int MaxWidth => 300;

        public override string Title => "Save " + (string.IsNullOrWhiteSpace(FileName) ? "Project" : FileName);

        public string InitialDirectory { get; internal set; }

        public string FileName { get; internal set; }

        public Action<string> SaveAction { get; internal set; }

        public void SaveProjectButtonClicked()
        {
            SaveAction(InitialDirectory + "\\" + FileName);
        }
    }
}