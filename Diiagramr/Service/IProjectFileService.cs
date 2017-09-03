using System.Collections.Generic;
using Diiagramr.Model;

namespace Diiagramr.Service
{
    public interface IProjectFileService
    {
        /// <summary>
        /// Creates a new project with the given name.
        /// </summary>
        /// <param name="name">The name of the new project.</param>
        /// <returns>The created project</returns>
        Project CreateProject(string name);

        /// <summary>
        /// Determines whether the specified name is valid.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>True if the name is valid for a new project</returns>
        bool IsProjectNameValid(string name);

        /// <summary>
        /// Saves the project.
        /// </summary>
        /// <param name="project">The project.</param>
        void SaveProject(Project project);

        /// <summary>
        /// Loads the project.
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        /// <returns>The loaded project.</returns>
        Project LoadProject(string projectName);

        /// <summary>
        /// Gets the saved project names.
        /// </summary>
        /// <returns>A List of the saved project names.</returns>
        IList<string> GetSavedProjectNames();

        /// <summary>
        /// Renames a project.
        /// </summary>
        /// <param name="oldName">The old name.</param>
        /// <param name="newName">The new name.</param>
        /// <returns>True is the rename was performed</returns>
        bool MoveProject(string oldName, string newName);
    }
}
