using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;
using System.Collections.Generic;

namespace VisualDrop
{
    public class NoveltyCurveNode : Node
    {
        private const int _frameCount = 60;
        private readonly List<float[]> _rawFrames = new List<float[]>();
        private readonly List<float[]> _differenceFrames = new List<float[]>();
        private readonly float[] _differenceFrameSums = new float[_frameCount];
        private int _currentFrame;

        public NoveltyCurveNode()
        {
            for (int i = 0; i < _frameCount; i++)
            {
                _rawFrames.Add(new float[0]);
                _differenceFrames.Add(new float[0]);
            }
            Width = 60;
            Height = 30;
            Name = "Novelty Curve";
        }

        [OutputTerminal(Direction.South)]
        public float[] NoveltyCurveOutput { get; set; }

        [InputTerminal(Direction.North)]
        public void Input(float[] data)
        {
            if (data == null || data.Length == 0)
            {
                return;
            }

            _currentFrame++;
            SetFrame(0, data);
            ComputeDifferenceFrame(0);
        }

        private void ComputeDifferenceFrame(int frameNumber)
        {
            var currentFrame = GetFrame(frameNumber);
            var lastFrame = GetFrame(frameNumber - 1);
            if (currentFrame.Length != lastFrame.Length)
            {
                return;
            }
            var differenceFrame = new float[currentFrame.Length];
            var differenceFrameSum = 0f;

            for (int i = 0; i < differenceFrame.Length; i++)
            {
                var difference = currentFrame[i] - lastFrame[i];
                differenceFrame[i] = difference > 0 ? difference : 0;
                differenceFrameSum += differenceFrame[i];
            }

            SetDifferenceFrame(frameNumber, differenceFrame);
            SetDifferenceFrameSum(frameNumber, differenceFrameSum);

            NoveltyCurveOutput = GetDifferenceFrame(0);
        }

        private void SetFrame(int frameIndex, float[] frame)
        {
            _rawFrames[frameIndex + _currentFrame % _frameCount] = frame;
        }

        private float[] GetFrame(int frameIndex)
        {
            var index = frameIndex + _currentFrame % _frameCount;
            if (index < 0)
            {
                index += _frameCount;
            }
            return _rawFrames[index];
        }

        private void SetDifferenceFrame(int frameIndex, float[] frame)
        {
            _differenceFrames[frameIndex + _currentFrame % _frameCount] = frame;
        }

        private float[] GetDifferenceFrame(int frameIndex)
        {
            return _differenceFrames[frameIndex + _currentFrame % _frameCount];
        }

        private void SetDifferenceFrameSum(int sumIndex, float sum)
        {
            _differenceFrameSums[sumIndex % _frameCount] = sum;
        }

        private float GetDifferenceFrameSum(int sumIndex)
        {
            return _differenceFrameSums[sumIndex % _frameCount];
        }
    }
}