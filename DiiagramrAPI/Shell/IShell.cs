using DiiagramrAPI.Service.Shell;
using DiiagramrAPI.Shell.ShellCommands;
using Stylet;
using System.Collections.Generic;
using System.Windows;

namespace DiiagramrAPI.Shell
{
    public interface IShell
    {
        IObservableCollection<TopLevelToolBarCommand> ToolBarItems { get; }

        void AttachToViewModel(ShellViewModel shellViewModel);

        void OpenWindow(AbstractShellWindow window);

        void SetWindowTitle(string title);

        void ShowContextMenu(IList<IShellCommand> commands, Point position);

        void ShowScreen(IScreen screen);
    }
}