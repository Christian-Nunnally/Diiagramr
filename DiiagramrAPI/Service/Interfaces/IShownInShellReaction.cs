namespace DiiagramrAPI.Service.Interfaces
{
    /// <summary>
    /// Interface that can be implemented by subclasses of <see cref="Screen"/> to allow them to react to being shown in the shell.
    /// </summary>
    internal interface IShownInShellReaction
    {
        /// <summary>
        /// Method that gets invoked when the shell is showing this object.
        /// </summary>
        void ShownInShell();
    }
}