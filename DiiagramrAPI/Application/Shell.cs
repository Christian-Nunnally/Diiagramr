using DiiagramrAPI.Application.Tools;
using DiiagramrAPI.Project;
using DiiagramrAPI.Service.Application;
using Stylet;
using System;
using System.ComponentModel;
using System.Reflection;

namespace DiiagramrAPI.Application
{
    public class Shell : Screen
    {
        public const double MaximizedWindowChromeRelativePositionAdjustment = -4;
        public const string StartCommandId = "start";

        public Shell(
            Func<IProjectManager> projectManagerFactory,
            Func<ContextMenu> contextMenuFactory,
            Func<ScreenHost> screenHostFactory,
            Func<DialogHost> dialogHostFactory,
            Func<Toolbar> toolbarFactory)
        {
            ProjectManager = projectManagerFactory();
            ContextMenu = contextMenuFactory();
            ScreenHost = screenHostFactory();
            DialogHost = dialogHostFactory();
            Toolbar = toolbarFactory();
            CommandExecutor.Execute(StartCommandId);
        }

        public double Width { get; set; } = 1010;

        public double Height { get; set; } = 830;

        public string WindowTitle { get; set; } = "Visual Drop - " + Assembly.GetEntryAssembly().GetName().Version.ToString(4);

        public IProjectManager ProjectManager { get; }

        public ContextMenu ContextMenu { get; set; }

        public Toolbar Toolbar { get; set; }

        public DialogHost DialogHost { get; set; }

        public ScreenHost ScreenHost { get; set; }

        public override void RequestClose(bool? dialogResult = null)
        {
            if (dialogResult == true)
            {
                return;
            }
            ProjectManager.CloseProject(() =>
            {
                if (Parent != null)
                {
                    base.RequestClose(dialogResult);
                }
            });
        }

        public void WindowClosing(object sender, CancelEventArgs e)
        {
            ProjectManager.CloseProject(() =>
            {
                RequestClose(true);
                ScreenHost.CloseCurrentScreens();
                BackgroundTaskManager.Instance.CancelAllTasks();
                System.Windows.Application.Current.Shutdown();
            });
            e.Cancel = true;
        }
    }
}