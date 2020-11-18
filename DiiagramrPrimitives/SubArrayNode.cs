using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;

namespace DiiagramrPrimitives
{
    /// <summary>
    /// A simple node that outputs a subsection of the array input to it.
    /// </summary>
    [Help("Outputs a subset of the input array, specified by the start and end index terminals.")]
    public class SubArrayNode : Node
    {
        private object[] _array;

        /// <summary>
        /// Creates a new instance of <see cref="SubArrayNode"/>.
        /// </summary>
        public SubArrayNode()
        {
            Width = 30;
            Height = 30;
            Name = "SubArray";
        }

        [Help("The start index of the output array in the original input array")]
        [NodeSetting]
        [InputTerminal(Direction.West)]
        public int StartIndex { get; set; }

        [Help("The end index of the output array in the original input array")]
        [NodeSetting]
        [InputTerminal(Direction.East)]
        public int Endndex { get; set; }

        [Help("The resulting array subsection of the input array.")]
        [OutputTerminal(Direction.South)]
        public object[] Result { get; set; }

        [Help("The array to copy a subsection of.")]
        [InputTerminal(Direction.North)]
        public object[] Input
        {
            set
            {
                _array = value;
                if (_array != null)
                {
                    if (StartIndex < _array.Length && Endndex < _array.Length && StartIndex < Endndex)
                    {
                        var newOutput = new object[Endndex - StartIndex];
                        for (int i = StartIndex; i < Endndex; i++)
                        {
                            newOutput[i] = _array[i];
                        }
                        Result = newOutput;
                    }
                }
            }
            get => _array;
        }
    }
}