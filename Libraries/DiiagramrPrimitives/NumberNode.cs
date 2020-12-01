using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;
using System.Windows;
using System.Windows.Input;

namespace DiiagramrPrimitives
{
    /// <summary>
    /// Simple node that allows users to input a value.
    /// </summary>
    [Help("Contains a single settable number that it outputs whenever it changes.")]
    public class NumberNode : Node
    {
        /// <summary>
        /// Creates a new instance of <see cref="NumberNode"/>.
        /// </summary>
        public NumberNode() : base()
        {
            Width = 30;
            Height = 30;
            Name = "Number";
            ResizeEnabled = true;
        }

        /// <summary>
        /// The output of the node.
        /// </summary>
        [Help("Outputs the value set in the node.")]
        [NodeSetting]
        [OutputTerminal(Direction.South)]
        public int Number { get; set; } = 64;

        /// <summary>
        /// Gets or sets the string version of the output.
        /// </summary>
        public string StringValue
        {
            get => Number.ToString();

            set
            {
                if (int.TryParse(value, out int result))
                {
                    Number = result;
                }
            }
        }

        /// <summary>
        /// Occurs when the user presses a key while the number text box has focus.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        public void PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && View != null)
            {
                (sender as FrameworkElement)?.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }
    }
}