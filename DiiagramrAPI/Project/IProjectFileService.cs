using DiiagramrAPI.Service;
using DiiagramrModel;
using System;

namespace DiiagramrAPI.Project
{
    public interface IProjectFileService : ISingletonService
    {
        event Action<ProjectModel> ProjectSaved;

        string ProjectDirectory { get; set; }

        /// <summary>
        /// Loads the project.
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
        /// Saves the project.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="saveAs">Whether this should be saved with saveAs.</param>
        void SaveProject(ProjectModel project, bool saveAs, Action continuation);
    }
}