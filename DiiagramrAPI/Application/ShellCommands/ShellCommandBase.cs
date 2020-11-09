using DiiagramrAPI.Service.Application;
using System;
using System.Collections.Generic;

namespace DiiagramrAPI.Application.ShellCommands
{
    /// <summary>
    /// Base class for all commands in DiiagramrAPI.
    /// </summary>
    public abstract class ShellCommandBase : IShellCommand
    {
        /// <summary>
        /// Invoked when an unhandled exception occurs while executing a command.
        /// </summary>
        public static event Action<Exception> OnShellCommandException;

        public abstract string Name { get; }

        public IList<IShellCommand> SubCommandItems { get; } = new List<IShellCommand>();

        public bool CachedCanExecute { get; set; }

        public bool CanExecute() => CachedCanExecute = CanExecuteInternal();

        public void Execute(object parameter)
        {
            try
            {
                if (CanExecute())
                {
                    ExecuteInternal(parameter);
                }
            }
            catch (Exception e)
            {
                OnShellCommandException?.Invoke(e);
            }
        }

        public void Execute() => Execute(null);

        protected abstract void ExecuteInternal(object parameter);

        protected abstract bool CanExecuteInternal();
    }
}