using DiiagramrAPI.Model;
using System.Windows;

namespace DiiagramrAPI.Service.Interfaces
{
    public interface IProjectFileService : IDiiagramrService
    {
        string ProjectDirectory { get; set; }

        /// <summary>
        /// Confirms ProjectModel Close.
        /// </summary>
        /// <returns>The Result from the calling Dialog.</returns>
        MessageBoxResult ConfirmProjectClose();

        /// <summary>
        /// Loads the project.
        /// </summary>
        /// <returns>The loaded project.</returns>
        ProjectModel LoadProject();

        /// <summary>
        /// Saves the project.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="saveAs">Whether this should be saved with saveAs.</param>
        bool SaveProject(ProjectModel project, bool saveAs);
    }
}
