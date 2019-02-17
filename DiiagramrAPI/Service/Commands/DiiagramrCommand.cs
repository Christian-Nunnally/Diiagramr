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

        public bool CanExecute(ShellViewModel shell)
        {
            return true;
        }

        internal abstract void ExecuteInternal(ShellViewModel shell, object parameter);

        public void Execute(ShellViewModel shell, object parameter)
        {
            if (CanExecute(shell))
            {
                ExecuteInternal(shell, parameter);
            }
        }
    }
}
