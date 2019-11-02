using DiiagramrAPI.Service.Interfaces;
using DiiagramrAPI.Shell;
using System.Collections.Generic;

namespace DiiagramrAPI.Shell.ShellCommands
{
    public interface IShellCommand : IService
    {
        string Name { get; }

        string Parent { get; }

        float Weight { get; }

        IList<IShellCommand> SubCommandItems { get; }

        bool CanExecute(IShell shell);

        void Execute(IShell shell, object parameter);
    }
}
