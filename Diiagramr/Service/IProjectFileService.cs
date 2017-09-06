using System.Collections.Generic;
using Diiagramr.Model;

namespace Diiagramr.Service
{
    public interface IProjectFileService
    {
        
        string ProjectName { get; set; }

        /// <summary>
        /// Saves the project.
        /// </summary>
        /// <param name="project">The project.</param>
        bool SaveProject(Project project, bool saveAs);

        /// <summary>
        /// Loads the project.
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        /// <returns>The loaded project.</returns>
        Project LoadProject();

        /// <summary>
        /// Renames a project.
        /// </summary>
        /// <param name="oldName">The old name.</param>
        /// <param name="newName">The new name.</param>
        /// <returns>True is the rename was performed</returns>
        bool MoveProject(string oldName, string newName);
    }
}
