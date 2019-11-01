using DiiagramrAPI.Shell.ShellCommands;
using Stylet;
using System.Collections.Generic;
using System.Windows;

namespace DiiagramrAPI.Shell
{
    public interface IShell
    {
        IObservableCollection<TopLevelToolBarCommand> ToolBarItems { get; set; }

        void AttachToViewModel(ShellViewModel shellViewModel);

        void ShowContextMenu(IList<IShellCommand> commands, Point position);

        void OpenWindow(AbstractShellWindow window);

        void ShowScreen(IScreen screen);

        void SetWindowTitle(string title);
    }
}
