using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;

namespace DiiagramrPrimitives
{
    public class SubArrayNode : Node
    {
        private object[] _array;

        public SubArrayNode()
        {
            Width = 30;
            Height = 30;
            Name = "SubArray";
        }

        [NodeSetting]
        [InputTerminal(Direction.West)]
        public int StartIndex { get; set; }

        [NodeSetting]
        [InputTerminal(Direction.East)]
        public int Endndex { get; set; }

        [OutputTerminal(Direction.South)]
        public object[] Value { get; set; }

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
                        Value = newOutput;
                    }
                }
            }
            get => _array;
        }
    }
}