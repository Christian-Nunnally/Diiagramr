using DiiagramrAPI.Application.ShellCommands;
using DiiagramrAPI.Service.Application;
using Stylet;
using System.Collections.Generic;
using System.Windows;

namespace DiiagramrAPI.Application
{
    public interface IApplicationShell
    {
        IObservableCollection<TopLevelToolBarCommand> ToolBarItems { get; }

        void AttachToViewModel(ShellViewModel shellViewModel);

        void OpenWindow(AbstractShellWindow window);

        void SetWindowTitle(string title);

        void ShowContextMenu(IList<IShellCommand> commands, Point position);

        void ShowScreen(IScreen screen);
    }
}