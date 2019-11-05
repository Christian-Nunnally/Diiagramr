using DiiagramrAPI.Application;
using System.Collections.Generic;

namespace DiiagramrAPI.Service.Application
{
    public interface IShellCommand : IService
    {
        string Name { get; }

        string Parent { get; }

        IList<IShellCommand> SubCommandItems { get; }

        float Weight { get; }

        bool CanExecute(IApplicationShell shell);

        void Execute(IApplicationShell shell, object parameter);
    }
}