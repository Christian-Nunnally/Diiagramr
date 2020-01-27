using System.Collections.Generic;

namespace DiiagramrAPI.Service.Application
{
    public interface IShellCommand : ISingletonService
    {
        string Name { get; }

        string Parent { get; }

        bool LastCanExecute { get; set; }

        IList<IShellCommand> SubCommandItems { get; }

        float Weight { get; }

        bool CanExecute();

        void Execute(object parameter);
    }
}