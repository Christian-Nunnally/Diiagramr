using DiiagramrAPI.Application.ShellCommands;
using DiiagramrAPI.Service;
using DiiagramrAPI.Service.Application;
using Stylet;
using System.Collections.Generic;
using System.Windows;

namespace DiiagramrAPI.Application
{
    public interface IApplicationShell : ISingletonService
    {
        IObservableCollection<TopLevelToolBarCommand> ToolBarItems { get; }

        void AttachToShell(ShellViewModel shell);

        void SetWindowTitle(string title);

        // TODO: Move this to its own control like DialogHost
        void ShowContextMenu(IList<IShellCommand> commands, Point position);

        void ShowScreen(IScreen screen);
    }
}