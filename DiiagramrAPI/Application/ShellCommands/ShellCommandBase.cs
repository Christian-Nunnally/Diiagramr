using DiiagramrAPI.Service.Application;
using System.Collections.Generic;

namespace DiiagramrAPI.Application.ShellCommands
{
    /// <summary>
    /// Base class for all commands in DiiagramrAPI.
    /// </summary>
    public abstract class ShellCommandBase : IShellCommand
    {
        public abstract string Name { get; }

        public IList<IShellCommand> SubCommandItems { get; } = new List<IShellCommand>();

        public bool CachedCanExecute { get; set; }

        public bool CanExecute() => CachedCanExecute = CanExecuteInternal();

        public void Execute(object parameter)
        {
            if (CanExecute())
            {
                ExecuteInternal(parameter);
            }
        }

        public void Execute() => Execute(null);

        protected abstract void ExecuteInternal(object parameter);

        protected abstract bool CanExecuteInternal();
    }
}