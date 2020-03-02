using DiiagramrAPI.Application.ShellCommands;
using DiiagramrAPI.Application.Tools;
using DiiagramrAPI.Project;
using DiiagramrAPI.Service.Application;
using Stylet;
using StyletIoC;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Input;

namespace DiiagramrAPI.Application
{
    public class Shell : Screen
    {
        private readonly IHotkeyCommander _hotkeyCommander;

        /// <summary>
        /// The core class of the UI. Hosts everything inside the application.
        /// </summary>
        /// <param name="startCommandFactory">Factory for a command that will perform the appropriate actions to initialize the application after the empty shell has loaded.</param>
        /// <param name="hotkeyCommanderFactory">Factory for a hotkey commander capable of intercepting global hotkey presses made inside the shell.</param>
        /// <param name="contextMenuFactory">Factory for the context menu the shell provides.</param>
        /// <param name="screenHostFactory">Factory for the host for all screens shown in the shell.</param>
        /// <param name="dialogHostFactory">Factory for the host for all dialogs shown in the shell.</param>
        /// <param name="toolbarFactory">Factory for the command toolbar at the top of the shell.</param>
        public Shell(
            [Inject(Key = "startCommand")] Func<ShellCommandBase> startCommandFactory,
            Func<IHotkeyCommander> hotkeyCommanderFactory,
            Func<ContextMenu> contextMenuFactory,
            Func<ScreenHost> screenHostFactory,
            Func<DialogHost> dialogHostFactory,
            Func<Toolbar> toolbarFactory)
        {
            _hotkeyCommander = hotkeyCommanderFactory();
            ContextMenu = contextMenuFactory();
            ScreenHost = screenHostFactory();
            DialogHost = dialogHostFactory();
            Toolbar = toolbarFactory();
            startCommandFactory().Execute();
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
            TryCloseShell();
        }

        public void WindowClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = !TryCloseShell();
        }

        public void PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = _hotkeyCommander.HandleHotkeyPress(e.Key);
        }

        private bool TryCloseShell()
        {
            ScreenHost.InteractivelyCloseAllScreens(() =>
            {
                RequestClose(true);
                BackgroundTaskManager.Instance.CancelAllTasks();
                View?.Dispatcher.Invoke(System.Windows.Application.Current.Shutdown);
            });
            return false;
        }
    }
}