using DiiagramrAPI.Project;
using System;
using System.Windows.Input;

namespace DiiagramrAPI.Application.ShellCommands.FileCommands
{
    /// <summary>
    /// A command that lets the user save the project with a given name and path.
    /// </summary>
    public class SaveAsProjectCommand : ShellCommandBase, IToolbarCommand, IHotkeyCommand
    {
        private readonly IProjectManager _projectManager;
        private readonly IProjectFileService _projectFileService;

        /// <summary>
        /// Creates a new instance of <see cref="SaveAsProjectCommand"/>
        /// </summary>
        /// <param name="projectManagerFactory">Factory to get an instance of an <see cref="IProjectManager"/>.</param>
        /// <param name="projectFileServiceFactory">Factory to get an instance of an <see cref="IProjectFileService"/>.</param>
        public SaveAsProjectCommand(
            Func<IProjectManager> projectManagerFactory,
            Func<IProjectFileService> projectFileServiceFactory)
        {
            _projectManager = projectManagerFactory();
            _projectFileService = projectFileServiceFactory();
        }

        /// <inheritdoc/>
        public override string Name => "Save As...";

        /// <inheritdoc/>
        public string ParentName => "Project";

        /// <inheritdoc/>
        public float Weight => .5f;

        /// <inheritdoc/>
        public Key Hotkey => Key.S;

        /// <inheritdoc/>
        public bool RequiresCtrlModifierKey => true;

        /// <inheritdoc/>
        public bool RequiresShiftModifierKey => true;

        /// <inheritdoc/>
        public bool RequiresAltModifierKey => false;

        /// <inheritdoc/>
        protected override void ExecuteInternal(object parameter)
        {
            if (_projectManager.Project != null)
            {
                _projectFileService.SaveProject(_projectManager.Project, saveAs: true, () => { });
            }
        }

        /// <inheritdoc/>
        protected override bool CanExecuteInternal()
        {
            return _projectManager.Project is object;
        }
    }
}