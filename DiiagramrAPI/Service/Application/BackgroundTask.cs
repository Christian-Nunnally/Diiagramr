using System;
using System.Threading;

namespace DiiagramrAPI.Service.Application
{
    public class BackgroundTask
    {
        private readonly Action _action;
        private readonly bool _repeatTask;
        private readonly int _repeatDelay;
        private bool _shouldCancel;

        public BackgroundTask(Action action)
        {
            _action = action;
        }

        public BackgroundTask(Action action, int repeatDelay)
        {
            _action = action;
            _repeatDelay = repeatDelay;
            _repeatTask = true;
        }

        public bool IsRunning { get; private set; }

        public bool Paused { get; set; }

        public void Cancel()
        {
            _shouldCancel = true;
        }

        public void Start()
        {
            Paused = false;
            new Thread(ThreadFunction).Start();
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