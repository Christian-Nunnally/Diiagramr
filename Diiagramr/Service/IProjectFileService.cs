using System.Collections.Generic;
using System.Windows.Forms;
using Diiagramr.Model;

namespace Diiagramr.Service
{
    public interface IProjectFileService
    {

        string ProjectDirectory { get; set; }

        /// <summary>
        /// Saves the project.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="saveAs">Whether this should be saved with saveAs.</param>
        bool SaveProject(Project project, bool saveAs);

        /// <summary>
        /// Loads the project.
        /// </summary>
        /// <returns>The loaded project.</returns>
        Project LoadProject();

        /// <summary>
        /// Confirms Project Close.
        /// </summary>
        /// <returns>The Result from the calling Dialog.</returns>
        DialogResult ConfirmProjectClose();
    }
}
