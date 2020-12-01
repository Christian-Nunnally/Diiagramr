using System;
using System.Threading;

namespace DiiagramrAPI.Service.Application
{
    /// <summary>
    /// A task that runs in the background.
    /// </summary>
    public class BackgroundTask
    {
        private readonly Action _action;
        private readonly bool _repeatTask;
        private readonly int _repeatDelay;
        private bool _shouldCancel;

        /// <summary>
        /// Creates a new instance of <see cref="BackgroundTask"/>.
        /// </summary>
        /// <param name="action">The action to run in the background.</param>
        public BackgroundTask(Action action)
        {
            _action = action;
        }

        /// <summary>
        /// Creates a new instance of <see cref="BackgroundTask"/>.
        /// </summary>
        /// <param name="action">The action to run in the background.</param>
        /// <param name="repeatDelay">The delay between repeating the action.</param>
        public BackgroundTask(Action action, int repeatDelay)
        {
            _action = action;
            _repeatDelay = repeatDelay;
            _repeatTask = true;
        }

        /// <summary>
        /// Gets whether the background task is currently running.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Gets whether the background task is currently paused.
        /// </summary>
        public bool Paused { get; set; }

        /// <summary>
        /// Cancels the background operation.
        /// </summary>
        public void Cancel()
        {
            _shouldCancel = true;
        }

        /// <summary>
        /// Starts the task in a background thread.
        /// </summary>
        /// <returns>The original <see cref="BackgroundTask"/>.</returns>
        public BackgroundTask Start()
        {
            Paused = false;
            new Thread(ThreadFunction).Start();
            return this;
        }

        private void ThreadFunction()
        {
            IsRunning = true;
            if (_repeatTask)
            {
                while (!_shouldCancel)
                {
                    if (!Paused)
                    {
                        _action();
                    }
                    Thread.Sleep(_repeatDelay);
                }
            }
            else
            {
                _action();
            }
            IsRunning = false;
        }
    }
}