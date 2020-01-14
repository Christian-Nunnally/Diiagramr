using DiiagramrAPI.Application.Tools;
using DiiagramrAPI.Project;
using DiiagramrAPI.Service.Application;
using DiiagramrModel2;
using Stylet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace DiiagramrAPI.Application
{
    public class ShellViewModel : Conductor<IScreen>.StackNavigation
    {
        public const double MaximizedWindowChromeRelativePositionAdjustment = -4;
        public const string StartCommandId = "start";
        private TimeSpan _lastMouseDownTime;

        public ShellViewModel(
            Func<IProjectManager> projectManagerFactory,
            Func<ContextMenu> contextMenuViewModelFactory,
            Func<IApplicationShell> shellFactory,
            Func<ToolbarViewModel> toolbarViewModelFactory)
        {
            Shell = shellFactory.Invoke();
            Shell.AttachToViewModel(this);
            ContextMenuViewModel = contextMenuViewModelFactory.Invoke();
            ProjectManager = projectManagerFactory.Invoke();
            ToolbarViewModel = toolbarViewModelFactory.Invoke();
            ShellCommand.Execute(StartCommandId);
            ValueCoersionHelper.InitializeDefaultCoersionFunctions();
        }

        public ShellWindow ActiveWindow { get; set; }

        public bool CanSaveAsProject { get; set; }

        public bool CanSaveProject { get; set; }

        public ContextMenu ContextMenuViewModel { get; set; }

        public double Height { get; set; } = 830;

        public bool IsWindowOpen => ActiveWindow != null;

        public IProjectManager ProjectManager { get; }

        public IApplicationShell Shell { get; }

        public ToolbarViewModel ToolbarViewModel { get; set; }

        public double Width { get; set; } = 1010;

        public Stack<ShellWindow> WindowStack { get; } = new Stack<ShellWindow>();

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

        public void OpenWindow(ShellWindow window)
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
            BackgroundTaskManager.Instance.CancelAllTasks();
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