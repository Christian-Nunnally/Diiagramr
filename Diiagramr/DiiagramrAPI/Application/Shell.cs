using DiiagramrAPI.Application.Dialogs;
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
    /// <summary>
    /// The main application shell. Responsible for hosting things that manage any subsystem in the application.
    /// </summary>
    public class Shell : Screen
    {
        private readonly IHotkeyHandler _hotkeyCommander;

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
            [Inject(Key = "startCommand")] Func<IShellCommand> startCommandFactory,
            Func<IHotkeyHandler> hotkeyCommanderFactory,
            Func<ContextMenuBase> contextMenuFactory,
            Func<ScreenHostBase> screenHostFactory,
            Func<DialogHostBase> dialogHostFactory,
            Func<ToolbarBase> toolbarFactory)
        {
            _hotkeyCommander = hotkeyCommanderFactory();
            ContextMenu = contextMenuFactory();
            ScreenHost = screenHostFactory();
            DialogHost = dialogHostFactory();
            Toolbar = toolbarFactory();
            startCommandFactory().Execute(null);

            ShellCommandBase.OnShellCommandException += OnShellCommandException;
        }

        /// <summary>
        /// Gets or sets the width of the shell.
        /// </summary>
        public double Width { get; set; } = 1010;

        /// <summary>
        /// Gets or sets the height of the shell.
        /// </summary>
        public double Height { get; set; } = 830;

        /// <summary>
        /// Gets or sets the window title of the shell.
        /// </summary>
        public string WindowTitle { get; set; } = "Visual Drop - " + Assembly.GetEntryAssembly().GetName().Version.ToString(4);

        /// <summary>
        /// Gets the project manager this shell is using.
        /// </summary>
        public IProjectManager ProjectManager { get; }

        /// <summary>
        /// Gets or sets the context menu this shell is using.
        /// </summary>
        public ContextMenuBase ContextMenu { get; set; }

        /// <summary>
        /// Gets or sets the toolbar of the shell.
        /// </summary>
        public ToolbarBase Toolbar { get; set; }

        /// <summary>
        /// Gets or sets the dialog host of the shell.
        /// </summary>
        public DialogHostBase DialogHost { get; set; }

        /// <summary>
        /// Gets or sets the screen host of the shell.
        /// </summary>
        public ScreenHostBase ScreenHost { get; set; }

        /// <inheritdoc/>
        public override void RequestClose(bool? dialogResult = null)
        {
            if (dialogResult == true)
            {
                return;
            }
            TryCloseShell();
        }

        /// <summary>
        /// Occurs when the shell window is closing.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        public void WindowClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = !TryCloseShell();
        }

        /// <summary>
        /// Occurs when a key is pressed in the shell.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        public void PreviewKeyDownHandler(object sender, KeyEventArgs e)
        {
            e.Handled = PreviewKeyDown(e.Key);
        }

        /// <summary>
        /// Handles key presses.
        /// </summary>
        /// <param name="key">The key press to handle.</param>
        /// <returns>True if the key press was handled.</returns>
        public bool PreviewKeyDown(Key key) => _hotkeyCommander.HandleHotkeyPress(key);

        private void OnShellCommandException(Exception exception)
        {
            var exceptionDialog = new ExceptionDialog(exception);
            DialogHost.ActiveDialog = exceptionDialog;
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