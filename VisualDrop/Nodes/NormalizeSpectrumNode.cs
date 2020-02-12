using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;
using System;

namespace VisualDrop
{
    public class NormalizeSpectrumNode : Node
    {
        private float[] _outputData = new float[1];

        public NormalizeSpectrumNode()
        {
            Width = 60;
            Height = 60;
            Name = "Logarithmic Fit";
        }

        public float FitFactor1 { get; set; } = 1.00f;

        public string FitFactor1TextValue
        {
            get => FitFactor1.ToString();
            set => FitFactor1 = float.TryParse(value, out _) ? float.Parse(value) : FitFactor1;
        }

        public string FitFactor1String => "FitFactor1 = " + FitFactor1.ToString("0.00");

        public float FitFactor2 { get; set; } = 10.0f;

        public string FitFactor2TextValue
        {
            get => FitFactor2.ToString();
            set => FitFactor2 = float.TryParse(value, out _) ? float.Parse(value) : FitFactor2;
        }

        public string FitFactor2String => "FitFactor2 = " + FitFactor2.ToString("0.00");

        [OutputTerminal(Direction.South)]
        public float[] LogResult { get; set; }

        [InputTerminal(Direction.North)]
        public void Input(float[] data)
        {
            if (data == null || data.Length == 0)
            {
                return;
            }
            if (_outputData.Length != data.Length)
            {
                _outputData = new float[data.Length];
            }
            for (var i = 0; i < data.Length; i++)
            {
                _outputData[i] = (float)Math.Log(data[i] * FitFactor1) * FitFactor2;
            }

            LogResult = null;
            LogResult = _outputData;
            _outputData = new float[data.Length];
        }
    }
}