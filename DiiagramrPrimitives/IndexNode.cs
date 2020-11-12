using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;
using System.Windows;
using System.Windows.Input;

namespace DiiagramrPrimitives
{
    public class IndexNode : Node
    {
        private object[] _array;

        public IndexNode()
        {
            Width = 30;
            Height = 30;
            Name = "Index";
            ResizeEnabled = true;
        }

        [NodeSetting]
        public int IndexValue { get; set; }

        [OutputTerminal(Direction.South)]
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

        [InputTerminal(Direction.North)]
        public int Index
        {
            get => IndexValue;
            set
            {
                IndexValue = value;
                OnPropertyChanged(nameof(StringValue));
            }
        }

        [InputTerminal(Direction.East)]
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

        public void PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && View != null)
            {
                (sender as FrameworkElement)?.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }
    }
}