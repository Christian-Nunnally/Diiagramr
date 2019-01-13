using DiiagramrAPI.Service.Interfaces;
using DiiagramrAPI.ViewModel;
using System.Collections.Generic;

namespace DiiagramrAPI.Service.Commands
{
    public interface IDiiagramrCommand : IDiiagramrService
    {
        IList<IDiiagramrCommand> SubCommandItems { get; set; }

        string Parent { get; }
        string Name { get; }
        float Weight { get; }

        void Execute(ShellViewModel shell);

        bool CanExecute(ShellViewModel shell);
    }
}
