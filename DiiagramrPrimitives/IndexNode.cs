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
                        Array(_array);
                    }
                }
            }
        }

        public void PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && View != null)
            {
                (sender as FrameworkElement)?.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

        [InputTerminal(Direction.North)]
        public void Index(int index)
        {
            IndexValue = index;
            OnPropertyChanged(nameof(StringValue));
        }

        [InputTerminal(Direction.East)]
        public void Array(object[] array)
        {
            _array = array;
            if (array != null)
            {
                if (IndexValue < array.Length)
                {
                    Value = array[IndexValue];
                }
            }
        }
    }
}