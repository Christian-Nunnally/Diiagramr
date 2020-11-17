using DiiagramrCore;

namespace DiiagramrAPI.Application.ShellCommands.HelpCommands
{
    /// <summary>
    /// Opens the wiki in the default web browser.
    /// </summary>
    public class OpenWikiCommand : ShellCommandBase, IToolbarCommand
    {
        /// <inheritdoc/>
        public override string Name => "Open Wiki";

        /// <inheritdoc/>
        public string ParentName => "Help";

        /// <inheritdoc/>
        public float Weight => 0.3f;

        /// <inheritdoc/>
        protected override bool CanExecuteInternal()
        {
            return true;
        }

        /// <inheritdoc/>
        protected override void ExecuteInternal(object parameter)
        {
            CoreUilities.GoToSite("https://github.com/Christian-Nunnally/visual-drop/wiki");
        }
    }
}