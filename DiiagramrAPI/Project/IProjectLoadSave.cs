using DiiagramrAPI.Service;
using DiiagramrModel;

namespace DiiagramrAPI.Project
{
    /// <summary>
    /// Interface for loading and saving a project.
    /// </summary>
    public interface IProjectLoadSave : ISingletonService
    {
        /// <summary>
        /// Loads a project from a file.
        /// </summary>
        /// <param name="fileName">The path to the project to load.</param>
        /// <returns>The loaded project.</returns>
        ProjectModel Load(string fileName);

        /// <summary>
        /// Saves a project to the specified location.
        /// </summary>
        /// <param name="project">The project to save.</param>
        /// <param name="fileName">The location to save the project at.</param>
        void Save(ProjectModel project, string fileName);
    }
}