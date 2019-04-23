using DiiagramrAPI.Service.Commands;
using DiiagramrAPI.Service.Interfaces;
using DiiagramrAPI.Shell.Tools;
using Stylet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace DiiagramrAPI.Shell
{
    public class ShellViewModel : Conductor<IScreen>.StackNavigation
    {
        public const string StartCommandId = "start";
        public Stack<AbstractShellWindow> WindowStack = new Stack<AbstractShellWindow>();
        private TimeSpan _lastMouseDownTime;
        public const double MaximizedWindowChromeRelativePositionAdjustment = -4;

        public ShellViewModel(
            Func<IProjectManager> projectManagerFactory,
            Func<IEnumerable<IDiiagramrCommand>> commandsFactory,
            Func<ContextMenuViewModel> contextMenuViewModelFactory,
            Func<IShell> shellFactory,
            Func<ToolbarViewModel> toolbarViewModelFactory)
        {
            Shell = shellFactory.Invoke();
            Shell.AttachToViewModel(this);
            ContextMenuViewModel = contextMenuViewModelFactory.Invoke();
            ProjectManager = projectManagerFactory.Invoke();
            ToolbarViewModel = toolbarViewModelFactory.Invoke();
            ShellCommand.Execute(StartCommandId);
        }

        public bool CanSaveAsProject { get; set; }
        public bool CanSaveProject { get; set; }
        public bool IsWindowOpen => ActiveWindow != null;
        public IProjectManager ProjectManager { get; }
        public ToolbarViewModel ToolbarViewModel { get; set; }
        public ContextMenuViewModel ContextMenuViewModel { get; set; }
        public AbstractShellWindow ActiveWindow { get; set; }
        public string WindowTitle { get; set; } = "Visual Drop - " + Assembly.GetEntryAssembly().GetName().Version.ToString(4);
        public double Width { get; set; } = 1010;
        public double Height { get; set; } = 830;
        public IShell Shell { get; }

        public void MouseDownHandler()
        {
            _lastMouseDownTime = DateTime.Now.TimeOfDay;
        }

        public void MouseUpHandler()
        {
            if (DateTime.Now.TimeOfDay.Subtract(_lastMouseDownTime).TotalMilliseconds < 700)
            {
                CloseWindow();
            }
        }

        public void CloseWindow()
        {
            if (ActiveWindow != null)
            {
                ActiveWindow.OpenWindow -= OpenWindow;
                ActiveWindow = WindowStack.Count > 0 ? WindowStack.Pop() : null;
            }
        }

        public void OpenWindow(AbstractShellWindow window)
        {
            window.OpenWindow += OpenWindow;
            if (ActiveWindow != null)
            {
                WindowStack.Push(ActiveWindow);
            }

            ActiveWindow = window;
        }

        public override void RequestClose(bool? dialogResult = null)
        {
            if (ProjectManager.CloseProject())
            {
                if (Parent != null)
                {
                    base.RequestClose(dialogResult);
                }
            }
        }

        public void ShowContextMenu(IList<IDiiagramrCommand> commands, Point position)
        {
            ContextMenuViewModel.ShowContextMenu(commands, position);
        }

        public void ShowContextMenu(IList<IDiiagramrCommand> commands)
        {
            ShowContextMenu(commands, new Point(0, 22));
        }

        public void ShowScreen(IScreen screen)
        {
            CloseCurrentScreens();
            ActiveItem = screen;
            if (screen is IShownInShellReaction reaction)
            {
                reaction.ShownInShell();
            }
        }

        private void CloseCurrentScreens()
        {
            while (ActiveItem != null)
            {
                ActiveItem.RequestClose();
            }
        }

        public void WindowClosing(object sender, CancelEventArgs e)
        {
            if (!ProjectManager.CloseProject())
            {
                e.Cancel = true;
            }
        }

        public void MouseDownHandled(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
    }
}
