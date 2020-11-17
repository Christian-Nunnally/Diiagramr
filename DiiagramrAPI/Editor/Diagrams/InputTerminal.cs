using DiiagramrModel;

namespace DiiagramrAPI.Editor.Diagrams
{
    /// <summary>
    /// A view model to represent input terminals.
    /// </summary>
    public class InputTerminal : Terminal
    {
        /// <summary>
        /// Creates a new instance of <see cref="InputTerminal"/>.
        /// </summary>
        /// <param name="inputTerminal">The input terminal model.</param>
        public InputTerminal(InputTerminalModel inputTerminal)
            : base(inputTerminal)
        {
        }

        /// <summary>
        /// Occurs when the input terminal visual is double clicked.
        /// </summary>
        public void TerminalDoubleClicked()
        {
            if (Model.Type == typeof(bool))
            {
                Data = !(bool)(Data ?? false);
            }
        }
    }
}