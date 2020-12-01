using DiiagramrAPI.Editor.Diagrams;
using DiiagramrAPI.Service;
using DiiagramrModel;
using Stylet;
using System;

namespace DiiagramrAPI.Project
{
    /// <summary>
    /// Manages a currently active project and provides an API for starting new projects.
    /// </summary>
    public interface IProjectManager : ISingletonService
    {
        /// <summary>
        /// Gets or sets the current project.
        /// </summary>
        ProjectModel Project { get; set; }

        /// <summary>
        /// Gets all of the diagrams in the current project.
        /// </summary>
        IObservableCollection<Diagram> Diagrams { get; }

        /// <summary>
        /// Creates a new project.
        /// </summary>
        /// <param name="contiuation">A continuation that is called after the project is created.</param>
        void CreateProject(Action contiuation);

        /// <summary>
        /// Sets the currently active project to an existing project.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="autoOpenDiagram">Whether to automatically open diagrams when the project is open.</param>
        void SetProject(ProjectModel project, bool autoOpenDiagram = false);

        /// <summary>
        /// Closes the current project, and calls the continuation if the project actually closes without getting interupted.
        /// </summary>
        /// <param name="contiuation">The continuation to run after the project is closed.</param>
        void CloseProject(Action contiuation);

        /// <summary>
        /// Inserts a new diagram into the current project.
        /// </summary>
        /// <param name="diagram">The new diagram.</param>
        void InsertDiagram(DiagramModel diagram);

        /// <summary>
        /// Removes a diagram from the project.
        /// </summary>
        /// <param name="diagram">The diagram to remove.</param>
        void RemoveDiagram(DiagramModel diagram);
    }
}