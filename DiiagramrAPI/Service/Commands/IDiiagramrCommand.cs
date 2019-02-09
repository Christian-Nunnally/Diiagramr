using DiiagramrAPI.Service.Interfaces;
using DiiagramrAPI.ViewModel;
using System.Collections.Generic;

namespace DiiagramrAPI.Service.Commands
{
    public interface IDiiagramrCommand : IDiiagramrService
    {
        string Name { get; }
        string Parent { get; }
        IList<IDiiagramrCommand> SubCommandItems { get; set; }
        float Weight { get; }
        bool ShowInMenu { get; }

        bool CanExecute(ShellViewModel shell);

        void Execute(ShellViewModel shell);
    }
}
