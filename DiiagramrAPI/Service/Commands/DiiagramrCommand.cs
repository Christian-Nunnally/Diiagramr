using DiiagramrAPI.Shell;
using DiiagramrAPI.ViewModel;
using System.Collections.Generic;

namespace DiiagramrAPI.Service.Commands
{
    public abstract class DiiagramrCommand : IDiiagramrCommand
    {
        public abstract string Name { get; }
        public virtual string Parent => null;
        public IList<IDiiagramrCommand> SubCommandItems { get; set; } = new List<IDiiagramrCommand>();
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
