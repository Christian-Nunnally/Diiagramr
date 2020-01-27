using DiiagramrAPI.Service.Application;
using System.Collections.Generic;

namespace DiiagramrAPI.Application.ShellCommands
{
    public abstract class ShellCommandBase : IShellCommand
    {
        public abstract string Name { get; }

        public virtual string Parent => null;

        public IList<IShellCommand> SubCommandItems { get; } = new List<IShellCommand>();

        public virtual float Weight => 0f;

        public bool LastCanExecute { get; set; }

        public bool CanExecute()
        {
            LastCanExecute = CanExecuteInternal();
            return LastCanExecute;
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