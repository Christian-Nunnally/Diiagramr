using DiiagramrAPI.Application;
using DiiagramrAPI.Project;
using System;

namespace DiiagramrAPI2.Application.Dialogs
{
    public class SaveProjectDialog : Dialog
    {
        public override int MaxHeight => 300;

        public override int MaxWidth => 300;

        public override string Title { get; set; } = "Save Project";

        public string InitialDirectory { get; internal set; }

        public string FileName { get; internal set; }

        public Action<string> SaveAction { get; internal set; }

        public void SaveProjectButtonClicked()
        {
            string path = !FileName.EndsWith(ProjectFileService.ProjectFileExtension)
                ? InitialDirectory + "\\" + FileName + ProjectFileService.ProjectFileExtension
                : InitialDirectory + "\\" + FileName;
            CloseDialog();
            SaveAction(path);
        }
    }
}