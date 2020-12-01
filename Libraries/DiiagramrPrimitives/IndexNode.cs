using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;
using System.Windows;
using System.Windows.Input;

namespace DiiagramrPrimitives
{
    /// <summary>
    /// A simple node that takes the value out of an array at a particular index.
    /// </summary>
    [Help("Output the value in the input array at the specified index.")]
    public class IndexNode : Node
    {
        private object[] _array;

        /// <summary>
        /// Creates a new instance of <see cref="IndexNode"/>.
        /// </summary>
        public IndexNode()
        {
            Width = 30;
            Height = 30;
            Name = "Index";
            ResizeEnabled = true;
        }

        [NodeSetting]
        [Help("Output the value in the input array at the specified index.")]
        public int IndexValue { get; set; }

        [OutputTerminal(Direction.South)]
        [Help("Output the value in the input array at the specified index.")]
        public object Value { get; set; }

        public string StringValue
        {
            get => IndexValue.ToString();

            set
            {
                if (int.TryParse(value, out int result))
                {
                    if (result >= 0)
                    {
                        IndexValue = result;
                        Array = _array;
                    }
                }
            }
        }

        [InputTerminal(Direction.East)]
        [Help("The index to take the value out of the input array.")]
        public int Index
        {
            get => IndexValue;
            set
            {
                IndexValue = value;
                OnPropertyChanged(nameof(StringValue));
            }
        }

        [InputTerminal(Direction.North)]
        [Help("The array to get a value out of.")]
        public object[] Array
        {
            set
            {
                _array = value;
                if (_array != null)
                {
                    if (IndexValue < _array.Length)
                    {
                        Value = _array[IndexValue];
                    }
                }
            }
            get => _array;
        }

        /// <summary>
        /// Occurs when the user presses a key while the index input textbox has focus.
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