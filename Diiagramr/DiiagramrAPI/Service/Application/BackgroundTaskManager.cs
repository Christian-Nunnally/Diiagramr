using System;
using System.Collections.Generic;

namespace DiiagramrAPI.Service.Application
{
    /// <summary>
    /// Manages all active background tasks so that they can be correctly canceled if the application is closing.
    /// </summary>
    public class BackgroundTaskManager
    {
        private List<BackgroundTask> _managedTasks = new List<BackgroundTask>();

        private BackgroundTaskManager()
        {
        }

        /// <summary>
        /// The static instance of the <see cref="BackgroundTaskManager"/>.
        /// </summary>
        public static BackgroundTaskManager Instance { get; set; } = new BackgroundTaskManager();

        /// <summary>
        /// Creates a new managed <see cref="BackgroundTask"/>.
        /// </summary>
        /// <param name="action">The action to perform in the background.</param>
        /// <returns>The newly created <see cref="BackgroundTask"/>.</returns>
        public BackgroundTask CreateBackgroundTask(Action action)
        {
            var task = new BackgroundTask(action);
            _managedTasks.Add(task);
            return task;
        }

        /// <summary>
        /// Creates a new managed <see cref="BackgroundTask"/>.
        /// </summary>
        /// <param name="action">The action to perform in the background.</param>
        /// <param name="repeatDelay">The delay to take because repeating the action.</param>
        /// <returns>The newly created <see cref="BackgroundTask"/>.</returns>
        public BackgroundTask CreateBackgroundTask(Action action, int repeatDelay)
        {
            var task = new BackgroundTask(action, repeatDelay);
            _managedTasks.Add(task);
            return task;
        }

        /// <summary>
        /// Stop all managed <see cref="BackgroundTask"/>s.
        /// </summary>
        public void CancelAllTasks()
        {
            foreach (var task in _managedTasks)
            {
                task.Cancel();
            }
        }
    }
}