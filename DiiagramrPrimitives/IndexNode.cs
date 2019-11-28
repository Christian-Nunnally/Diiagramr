using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;
using System.Windows;
using System.Windows.Input;

namespace DiiagramrPrimitives
{
    public class IndexNode : Node
    {
        private byte[] _array;

        public IndexNode()
        {
            Width = 60;
            Height = 60;
            Name = "Index";
            ResizeEnabled = true;
        }

        [NodeSetting]
        public int Index { get; set; }

        [OutputTerminal(nameof(Value), Direction.South)]
        public byte Value { get; set; }

        public string StringValue
        {
            get => Index.ToString();

            set
            {
                if (int.TryParse(value, out int result))
                {
                    if (result >= 0)
                    {
                        Index = result;
                        ArrayInput(_array);
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

        [InputTerminal("Array", Direction.North)]
        public void ArrayInput(byte[] array)
        {
            _array = array;
            if (array != null)
            {
                if (Index < array.Length)
                {
                    Output(array[Index], nameof(Value));
                }
            }
        }
    }
}