using System.Windows.Forms;
using Diiagramr.Model;

namespace Diiagramr.Service.Interfaces
{
    public interface IProjectFileService
    {

        string ProjectDirectory { get; set; }

        /// <summary>
        /// Saves the project.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="saveAs">Whether this should be saved with saveAs.</param>
        bool SaveProject(ProjectModel project, bool saveAs);

        /// <summary>
        /// Loads the project.
        /// </summary>
        /// <returns>The loaded project.</returns>
        ProjectModel LoadProject();

        /// <summary>
        /// Confirms ProjectModel Close.
        /// </summary>
        /// <returns>The Result from the calling Dialog.</returns>
        DialogResult ConfirmProjectClose();
    }
}
