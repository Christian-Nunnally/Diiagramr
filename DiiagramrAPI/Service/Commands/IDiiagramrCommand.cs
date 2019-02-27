﻿using DiiagramrAPI.Service.Interfaces;
using DiiagramrAPI.ViewModel;
using System.Collections.Generic;

namespace DiiagramrAPI.Service.Commands
{
    public interface IDiiagramrCommand : IService
    {
        string Name { get; }
        string Parent { get; }
        IList<IDiiagramrCommand> SubCommandItems { get; set; }
        float Weight { get; }

        bool CanExecute(ShellViewModel shell);

        void Execute(ShellViewModel shell, object parameter);
    }
}
