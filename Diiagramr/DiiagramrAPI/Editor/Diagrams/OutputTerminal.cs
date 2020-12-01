using DiiagramrModel;

namespace DiiagramrAPI.Editor.Diagrams
{
    /// <summary>
    /// A view model for <see cref="OutputTerminalModel"/>.
    /// </summary>
    public class OutputTerminal : Terminal
    {
        /// <summary>
        /// Creates a new instance of <see cref="OutputTerminal"/>.
        /// </summary>
        /// <param name="outputTerminal">The terminal model.</param>
        public OutputTerminal(OutputTerminalModel outputTerminal)
            : base(outputTerminal)
        {
        }
    }
}