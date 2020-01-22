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

namespace DiiagramrAPI.Application
{
    public class ShellViewModel : Conductor<IScreen>.StackNavigation
    {
        public const double MaximizedWindowChromeRelativePositionAdjustment = -4;
        public const string StartCommandId = "start";

        public ShellViewModel(
            Func<IProjectManager> projectManagerFactory,
            Func<ContextMenu> contextMenuViewModelFactory,
            Func<IApplicationShell> shellFactory,
            Func<ToolbarViewModel> toolbarViewModelFactory,
            Func<DialogHost> dialogHostFactory)
        {
            Shell = shellFactory();
            Shell.AttachToShell(this);
            ContextMenuViewModel = contextMenuViewModelFactory();
            ProjectManager = projectManagerFactory();
            ToolbarViewModel = toolbarViewModelFactory();
            DialogHost = dialogHostFactory();
            ShellCommand.Execute(StartCommandId);
            ValueCoersionHelper.InitializeDefaultCoersionFunctions();
        }

        public bool CanSaveAsProject { get; set; }

        public bool CanSaveProject { get; set; }

        public double Width { get; set; } = 1010;

        public double Height { get; set; } = 830;

        public string WindowTitle { get; set; } = "Visual Drop - " + Assembly.GetEntryAssembly().GetName().Version.ToString(4);

        public ContextMenu ContextMenuViewModel { get; set; }

        public IProjectManager ProjectManager { get; }

        public IApplicationShell Shell { get; }

        public ToolbarViewModel ToolbarViewModel { get; set; }

        public DialogHost DialogHost { get; set; }

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
            CloseCurrentScreens();
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