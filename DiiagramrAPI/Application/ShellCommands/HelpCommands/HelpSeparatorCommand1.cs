namespace DiiagramrAPI.Application.ShellCommands
{
    /// <summary>
    /// A seperator in the toolbar menu.
    /// </summary>
    public class HelpSeparatorCommand1 : SeparatorCommand
    {
        /// <inheritdoc/>
        public override string ParentName => "Tools";

        /// <inheritdoc/>
        public override float Weight => 0.11f;
    }
}