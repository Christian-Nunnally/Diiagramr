using DiiagramrAPI.Project;
using System;
using System.Windows.Input;

namespace DiiagramrAPI.Application.ShellCommands.FileCommands
{
    /// <summary>
    /// A command that saves the current project.
    /// </summary>
    public class SaveProjectCommand : ShellCommandBase, IToolbarCommand, IHotkeyCommand
    {
        private readonly IProjectManager _projectManager;
        private readonly IProjectFileService _projectFileService;

        /// <summary>
        /// Creates a new instance of <see cref="SaveProjectCommand"/>
        /// </summary>
        /// <param name="projectManagerFactory">Factory to get an instance of an <see cref="IProjectManager"/>.</param>
        /// <param name="projectFileServiceFactory">Factory to get an instance of an <see cref="IProjectFileService"/>.</param>
        public SaveProjectCommand(
            Func<IProjectManager> projectManagerFactory,
            Func<IProjectFileService> projectFileServiceFactory)
        {
            _projectManager = projectManagerFactory();
            _projectFileService = projectFileServiceFactory();
        }

        /// <inheritdoc/>
        public override string Name => "Save";

        /// <inheritdoc/>
        public string ParentName => "Project";

        /// <inheritdoc/>
        public float Weight => .4f;

        /// <inheritdoc/>
        public Key Hotkey => Key.S;

        /// <inheritdoc/>
        public bool RequiresCtrlModifierKey => true;

        /// <inheritdoc/>
        public bool RequiresShiftModifierKey => false;

        /// <inheritdoc/>
        public bool RequiresAltModifierKey => false;

        /// <inheritdoc/>
        protected override void ExecuteInternal(object parameter)
        {
            if (_projectManager.Project != null)
            {
                _projectFileService.SaveProject(_projectManager.Project, saveAs: false, () => { });
            }
        }

        /// <inheritdoc/>
        protected override bool CanExecuteInternal()
        {
            return _projectManager.Project is object;
        }
    }
}