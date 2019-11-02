using DiiagramrAPI.Service.Application;
using System.Collections.Generic;

namespace DiiagramrAPI.Application.ShellCommands
{
    public abstract class DiiagramrCommand : IShellCommand
    {
        public abstract string Name { get; }
        public virtual string Parent => null;
        public IList<IShellCommand> SubCommandItems { get; } = new List<IShellCommand>();
        public virtual float Weight => 0f;

        public bool CanExecute(IApplicationShell shell)
        {
            return true;
        }

        public void Execute(IApplicationShell shell, object parameter)
        {
            if (CanExecute(shell))
            {
                ExecuteInternal(shell, parameter);
            }
        }

        internal abstract void ExecuteInternal(IApplicationShell shell, object parameter);
    }
}