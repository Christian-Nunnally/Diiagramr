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

        /// <inheritdoc/>
        public abstract string Name { get; }

        public IList<IShellCommand> SubCommandItems { get; } = new List<IShellCommand>();

        /// <inheritdoc/>
        public bool CachedCanExecute { get; set; }

        /// <inheritdoc/>
        public bool CanExecute() => CachedCanExecute = CanExecuteInternal();

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public void Execute() => Execute(null);

        /// <inheritdoc/>
        protected abstract void ExecuteInternal(object parameter);

        /// <inheritdoc/>
        protected abstract bool CanExecuteInternal();
    }
}