using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;

namespace VisualDrop
{
    public class ExponentialMovingAverageNode : Node
    {
        private byte[] _lastData = new byte[0];

        public ExponentialMovingAverageNode()
        {
            Width = 90;
            Height = 30;
            Name = "Moving Average(EMA)";
        }

        public float LastDataWeight { get; set; }

        public string WeightString => "Weight = " + LastDataWeight.ToString("0.00");

        [OutputTerminal("Data Out", Direction.South)]
        public byte[] OutputData { get; set; }

        [InputTerminal("Data In", Direction.North)]
        public void InputData(byte[] data)
        {
            if (data == null)
            {
                return;
            }

            if (_lastData.Length != data.Length)
            {
                _lastData = new byte[data.Length];
            }

            for (var i = 0; i < data.Length; i++)
            {
                _lastData[i] = (byte)((data[i] * LastDataWeight) + (_lastData[i] * (1.0 - LastDataWeight)));
            }

            OutputData = null;
            OutputData = _lastData;
        }
    }
}