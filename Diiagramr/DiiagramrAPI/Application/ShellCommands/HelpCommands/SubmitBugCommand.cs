using DiiagramrCore;

namespace DiiagramrAPI.Application.ShellCommands.HelpCommands
{
    /// <summary>
    /// Opens the web form for submitting bugs.
    /// </summary>
    public class SubmitBugCommand : ShellCommandBase, IToolbarCommand
    {
        /// <inheritdoc/>
        public override string Name => "Submit Bug";

        /// <inheritdoc/>
        public float Weight => 0.39f;

        /// <inheritdoc/>
        public string ParentName => "Help";

        /// <inheritdoc/>
        protected override bool CanExecuteInternal()
        {
            return true;
        }

        /// <inheritdoc/>
        protected override void ExecuteInternal(object parameter)
        {
            CoreUilities.GoToSite("https://github.com/Christian-Nunnally/visual-drop/issues");
        }
    }
}