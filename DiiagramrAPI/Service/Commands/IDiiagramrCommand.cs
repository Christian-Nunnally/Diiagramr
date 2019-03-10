using DiiagramrAPI.Service.Interfaces;
using DiiagramrAPI.Shell;
using System.Collections.Generic;

namespace DiiagramrAPI.Service.Commands
{
    public interface IDiiagramrCommand : IService
    {
        string Name { get; }
        string Parent { get; }
        IList<IDiiagramrCommand> SubCommandItems { get; set; }
        float Weight { get; }

        bool CanExecute(IShell shell);

        void Execute(IShell shell, object parameter);
    }
}
