using DiiagramrAPI.Service.Application;
using System.Collections.Generic;

namespace DiiagramrAPI.Application.ShellCommands
{
    public abstract class ShellCommandBase : IShellCommand
    {
        public abstract string Name { get; }

        public IList<IShellCommand> SubCommandItems { get; } = new List<IShellCommand>();

        public bool CachedCanExecute { get; set; }

        public bool CanExecute()
        {
            CachedCanExecute = CanExecuteInternal();
            return CachedCanExecute;
        }

        public void Execute(object parameter)
        {
            if (CanExecute())
            {
                ExecuteInternal(parameter);
            }
        }

        protected abstract void ExecuteInternal(object parameter);

        protected abstract bool CanExecuteInternal();
    }
}