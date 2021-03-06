using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;
using System;

namespace VisualDrop
{
    public class NoveltyCurveNode : Node
    {
        private float[] _lastFrame = new float[0];
        private float[] _currentFrame = new float[0];

        public NoveltyCurveNode()
        {
            Width = 30;
            Height = 30;
            Name = "Novelty Curve";
        }

        [OutputTerminal(Direction.South)]
        public float[] NoveltyCurveOutput { get; set; }

        [InputTerminal(Direction.North)]
        public float[] Input
        {
            set
            {
                if (value == null || value.Length == 0)
                {
                    return;
                }

                _lastFrame = _currentFrame;
                _currentFrame = value;
                NoveltyCurveOutput = ComputeDifference();
            }
            get => _currentFrame;
        }

        private float[] ComputeDifference()
        {
            var smallest = Math.Min(_lastFrame.Length, _currentFrame.Length);
            var differenceFrame = new float[smallest];
            for (int i = 0; i < smallest; i++)
            {
                var difference = _currentFrame[i] - _lastFrame[i];
                differenceFrame[i] = difference > 0 ? difference : 0;
            }
            return differenceFrame;
        }
    }
}