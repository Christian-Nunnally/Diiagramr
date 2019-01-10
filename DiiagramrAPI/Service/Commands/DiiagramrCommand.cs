using DiiagramrAPI.ViewModel;
using System.Collections.Generic;

namespace DiiagramrAPI.Service.Commands
{
    public abstract class DiiagramrCommand : IDiiagramrCommand
    {
        public virtual string Parent => null;
        public abstract string Name { get; }
        public IList<IDiiagramrCommand> SubCommandItems { get; set; } = new List<IDiiagramrCommand>();
        public virtual float Weight => 0f;

        public bool CanExecute(ShellViewModel shell)
        {
            return true;
        }

        public abstract void Execute(ShellViewModel shell);
    }
}
