using System;
using System.Collections.Generic;

namespace DiiagramrAPI.Service.Application
{
    public class BackgroundTaskManager
    {
        private List<BackgroundTask> _managedTasks = new List<BackgroundTask>();

        private BackgroundTaskManager()
        {
        }

        public static BackgroundTaskManager Instance { get; set; } = new BackgroundTaskManager();

        public BackgroundTask CreateBackgroundTask(Action action)
        {
            var task = new BackgroundTask(action);
            _managedTasks.Add(task);
            return task;
        }

        public BackgroundTask CreateBackgroundTask(Action action, int repeatDelay)
        {
            var task = new BackgroundTask(action, repeatDelay);
            _managedTasks.Add(task);
            return task;
        }

        public void CancelAllTasks()
        {
            foreach (var task in _managedTasks)
            {
                task.Cancel();
            }
        }
    }
}