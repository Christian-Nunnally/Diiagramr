using DiiagramrAPI.Service;
using DiiagramrModel;
using System;

namespace DiiagramrAPI.Project
{
    /// <summary>
    /// Interface for loading and saving a project to a file system.
    /// </summary>
    public interface IProjectFileService : ISingletonService
    {
        /// <summary>
        /// Occurs when a project is saved.
        /// </summary>
        event Action<ProjectModel> ProjectSaved;

        /// <summary>
        /// The directory that the project should be saved in.
        /// </summary>
        string ProjectDirectory { get; set; }

        /// <summary>
        /// Loads the project using a continuation if user input is required.
        /// </summary>
        /// <returns>The loaded project.</returns>
        void LoadProject(Action<ProjectModel> continuation);

        /// <summary>
        /// Loads the project.
        /// </summary>
        /// <param name="path">The path to the project.</param>
        /// <returns>The loaded project.</returns>
        ProjectModel LoadProject(string path);

        /// <summary>
        /// Saves the project using a continuation if user input is required.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="saveAs">Whether the user should be prompted for a project name.</param>
        void SaveProject(ProjectModel project, bool saveAs, Action continuation);
    }
}