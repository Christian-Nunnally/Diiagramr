namespace DiiagramrAPI.Application.ShellCommands
{
    /// <summary>
    /// A seperator in the toolbar menu.
    /// </summary>
    public class ToolSeparatorCommand1 : SeparatorCommand
    {
        /// <inheritdoc/>
        public override string ParentName => "Help";

        /// <inheritdoc/>
        public override float Weight => 0.35f;
    }
}