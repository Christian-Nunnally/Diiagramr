using DiiagramrAPI.Service.Shell;
using System.Collections.Generic;

namespace DiiagramrAPI.Shell.ShellCommands
{
    public abstract class DiiagramrCommand : IShellCommand
    {
        public abstract string Name { get; }
        public virtual string Parent => null;
        public IList<IShellCommand> SubCommandItems { get; } = new List<IShellCommand>();
        public virtual float Weight => 0f;

        public bool CanExecute(IShell shell)
        {
            return true;
        }

        public void Execute(IShell shell, object parameter)
        {
            if (CanExecute(shell))
            {
                ExecuteInternal(shell, parameter);
            }
        }

        internal abstract void ExecuteInternal(IShell shell, object parameter);
    }
}