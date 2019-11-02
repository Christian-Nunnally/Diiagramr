using DiiagramrAPI.Project;
using DiiagramrAPI.Service.Shell;
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
        public const double MaximizedWindowChromeRelativePositionAdjustment = -4;
        public const string StartCommandId = "start";
        private TimeSpan _lastMouseDownTime;

        public ShellViewModel(
            Func<IProjectManager> projectManagerFactory,
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

        public AbstractShellWindow ActiveWindow { get; set; }
        public bool CanSaveAsProject { get; set; }
        public bool CanSaveProject { get; set; }
        public ContextMenuViewModel ContextMenuViewModel { get; set; }
        public double Height { get; set; } = 830;
        public bool IsWindowOpen => ActiveWindow != null;
        public IProjectManager ProjectManager { get; }
        public IShell Shell { get; }
        public ToolbarViewModel ToolbarViewModel { get; set; }
        public double Width { get; set; } = 1010;
        public Stack<AbstractShellWindow> WindowStack { get; } = new Stack<AbstractShellWindow>();
        public string WindowTitle { get; set; } = "Visual Drop - " + Assembly.GetEntryAssembly().GetName().Version.ToString(4);

        public void CloseWindow()
        {
            if (ActiveWindow != null)
            {
                ActiveWindow.OpenWindow -= OpenWindow;
                ActiveWindow = WindowStack.Count > 0 ? WindowStack.Pop() : null;
            }
        }

        public void MouseDownHandled(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

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

        public void ShowContextMenu(IList<IShellCommand> commands, Point position)
        {
            ContextMenuViewModel.ShowContextMenu(commands, position);
        }

        public void ShowContextMenu(IList<IShellCommand> commands)
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

        public void WindowClosing(object sender, CancelEventArgs e)
        {
            if (!ProjectManager.CloseProject())
            {
                e.Cancel = true;
            }
        }

        private void CloseCurrentScreens()
        {
            while (ActiveItem != null)
            {
                ActiveItem.RequestClose();
            }
        }
    }
}