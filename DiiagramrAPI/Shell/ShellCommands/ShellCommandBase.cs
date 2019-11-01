using DiiagramrAPI.Shell;
using System.Collections.Generic;

namespace DiiagramrAPI.Service.Commands
{
    public abstract class DiiagramrCommand : IShellCommand
    {
        public abstract string Name { get; }
        public virtual string Parent => null;
        public IList<IShellCommand> SubCommandItems { get; set; } = new List<IShellCommand>();
        public virtual float Weight => 0f;

        public bool CanExecute(IShell shell)
        {
            return true;
        }

        internal abstract void ExecuteInternal(IShell shell, object parameter);

        public void Execute(IShell shell, object parameter)
        {
            if (CanExecute(shell))
            {
                ExecuteInternal(shell, parameter);
            }
        }
    }
}
