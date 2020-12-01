namespace DiiagramrAPI.Application.ShellCommands
{
    /// <summary>
    /// A seperator in the toolbar menu.
    /// </summary>
    public class FileSeparatorCommand1 : SeparatorCommand
    {
        /// <inheritdoc/>
        public override string ParentName => "Project";

        /// <inheritdoc/>
        public override float Weight => 0.3f;
    }
}