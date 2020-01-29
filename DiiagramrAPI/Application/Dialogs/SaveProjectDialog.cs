using DiiagramrAPI.Application;
using DiiagramrAPI.Project;
using System;

namespace DiiagramrAPI2.Application.Dialogs
{
    public class SaveProjectDialog : Dialog
    {
        public SaveProjectDialog()
        {
            CommandBarCommands.Add(new DialogCommandBarCommand("Save", Save));
        }

        public override int MaxHeight => 110;

        public override int MaxWidth => 290;

        public override string Title { get; set; } = "Save Project";

        public string InitialDirectory { get; set; }

        public string ProjectName { get; set; }

        public Action<string> SaveAction { get; set; }

        public void Save()
        {
            string path = !ProjectName.EndsWith(ProjectFileService.ProjectFileExtension)
                ? InitialDirectory + "\\" + ProjectName + ProjectFileService.ProjectFileExtension
                : InitialDirectory + "\\" + ProjectName;
            CloseDialog();
            SaveAction(path);
        }
    }
}