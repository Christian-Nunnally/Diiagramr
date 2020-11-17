using DiiagramrAPI.Project;
using System;
using System.Windows.Input;

namespace DiiagramrAPI.Application.ShellCommands.FileCommands
{
    /// <summary>
    /// Opens the start screen for the visual drop plugin.
    /// </summary>
    public class VisualDropCloseProjectCommand : ShellCommandBase, IToolbarCommand, IHotkeyCommand
    {
        private readonly IProjectManager _projectManager;
        private readonly VisualDropStartScreen _startScreen;
        private readonly ScreenHostBase _screenHost;

        /// <summary>
        /// Creates a new instance of <see cref="VisualDropCloseProjectCommand"/>
        /// </summary>
        /// <param name="startScreenFactory">Factory to get an instance of an <see cref="VisualDropStartScreen"/>.</param>
        /// <param name="projectManagerFactory">Factory to get an instance of an <see cref="IProjectManager"/>.</param>
        /// <param name="screenHostFactory">Factory to get an instance of an <see cref="ScreenHostBase"/>.</param>
        public VisualDropCloseProjectCommand(
            Func<VisualDropStartScreen> startScreenFactory,
            Func<IProjectManager> projectManagerFactory,
            Func<ScreenHostBase> screenHostFactory)
        {
            _startScreen = startScreenFactory();
            _projectManager = projectManagerFactory();
            _screenHost = screenHostFactory();
        }

        /// <inheritdoc/>
        public override string Name => "Close";

        /// <inheritdoc/>
        public string ParentName => "Project";

        /// <inheritdoc/>
        public float Weight => 1.0f;

        /// <inheritdoc/>
        public Key Hotkey => Key.W;

        /// <inheritdoc/>
        public bool RequiresCtrlModifierKey => true;

        /// <inheritdoc/>
        public bool RequiresShiftModifierKey => true;

        /// <inheritdoc/>
        public bool RequiresAltModifierKey => false;

        /// <inheritdoc/>
        protected override void ExecuteInternal(object parameter)
        {
            _projectManager.CloseProject(() => _screenHost.ShowScreen(_startScreen));
        }

        /// <inheritdoc/>
        protected override bool CanExecuteInternal()
        {
            return _projectManager.Project is object;
        }
    }
}