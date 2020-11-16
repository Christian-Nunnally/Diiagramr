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

        /// <inheritdoc/>
        public override int MaxHeight => 110;

        /// <inheritdoc/>
        public override int MaxWidth => 290;

        /// <inheritdoc/>
        public override string Title { get; set; } = "Save Project";

        /// <summary>
        /// The initial directory to take users to when prompting them for a save location.
        /// </summary>
        public string InitialDirectory { get; set; }

        /// <summary>
        /// The string the user has entered as the project save name.
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// An action that will save the current project at the given path.
        /// </summary>
        public Action<string> SaveAction { get; set; }

        private void Save()
        {
            string path = !ProjectName.EndsWith(ProjectFileService.ProjectFileExtension)
                ? InitialDirectory + "\\" + ProjectName + ProjectFileService.ProjectFileExtension
                : InitialDirectory + "\\" + ProjectName;
            CloseDialog();
            SaveAction(path);
        }
    }
}