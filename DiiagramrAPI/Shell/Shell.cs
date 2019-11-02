using DiiagramrAPI.Shell.ShellCommands;
using Stylet;
using System;
using System.Collections.Generic;
using System.Windows;

namespace DiiagramrAPI.Shell
{
    /// <summary>
    /// Wrapper around <see cref="ShellViewModel"/> that exists in the composition container.
    /// Because an instance of this class can be retrived from the composition, small API's can be made for clients of the shell (for example <see cref="CommandManager"/>).
    /// </summary>
    public class Shell : IShell
    {
        private readonly Queue<Action<ShellViewModel>> _workQueue = new Queue<Action<ShellViewModel>>();
        private ShellViewModel _shellViewModel;
        public IObservableCollection<TopLevelToolBarCommand> ToolBarItems { get; } = new BindableCollection<TopLevelToolBarCommand>();

        public void AttachToViewModel(ShellViewModel shellViewModel)
        {
            _shellViewModel = shellViewModel;
            ExecuteAllActions();
        }

        public void OpenWindow(AbstractShellWindow window)
        {
            ExecuteWhenAttached(shell => shell.OpenWindow(window));
        }

        public void SetWindowTitle(string title)
        {
            ExecuteWhenAttached(shell => shell.WindowTitle = title);
        }

        public void ShowContextMenu(IList<IShellCommand> commands, Point position)
        {
            ExecuteWhenAttached(shell => shell.ShowContextMenu(commands, position));
        }

        public void ShowScreen(IScreen screen)
        {
            ExecuteWhenAttached(shell => shell.ShowScreen(screen));
        }

        private void ExecuteAllActions()
        {
            while (_workQueue.Count != 0)
            {
                _workQueue.Dequeue().Invoke(_shellViewModel);
            }
        }

        private void ExecuteWhenAttached(Action<ShellViewModel> shellAction)
        {
            _workQueue.Enqueue(shellAction);
            if (_shellViewModel != null)
            {
                ExecuteAllActions();
            }
        }
    }
}