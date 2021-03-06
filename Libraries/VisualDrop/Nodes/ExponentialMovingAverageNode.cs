using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;
using System;

namespace VisualDrop
{
    public class ExponentialMovingAverageNode : Node
    {
        private float[] _lastData = new float[0];

        public ExponentialMovingAverageNode()
        {
            Width = 60;
            Height = 30;
            Name = "Exponential Moving Average";
        }

        public int TimePeriods { get; set; } = 2;

        public string TimePeriodTextValue
        {
            get => TimePeriods.ToString();
            set => TimePeriods = int.TryParse(value, out _) ? int.Parse(value) : TimePeriods;
        }

        [OutputTerminal(Direction.South)]
        public float[] EMA { get; set; }

        [InputTerminal(Direction.North)]
        public float[] NewData
        {
            get => _lastData;
            set
            {
                if (value == null)
                {
                    return;
                }

                if (_lastData.Length != value.Length)
                {
                    _lastData = new float[value.Length];
                }

                for (var i = 0; i < value.Length; i++)
                {
                    var smoothingConstant = 2.0f / (TimePeriods + 1.0f);
                    var ema = ((value[i] - _lastData[i]) * smoothingConstant) + _lastData[i];
                    _lastData[i] = Math.Max(0, ema);
                }

                EMA = null;
                EMA = _lastData;
            }
        }
    }
}