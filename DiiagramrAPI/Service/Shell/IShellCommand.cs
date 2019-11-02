using DiiagramrAPI.Shell;
using System.Collections.Generic;

namespace DiiagramrAPI.Service.Shell
{
    public interface IShellCommand : IService
    {
        string Name { get; }

        string Parent { get; }

        IList<IShellCommand> SubCommandItems { get; }
        float Weight { get; }

        bool CanExecute(IShell shell);

        void Execute(IShell shell, object parameter);
    }
}