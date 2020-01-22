using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;
using System;
using System.Collections.Generic;

namespace VisualDrop
{
    public class NoveltyCurveNode : Node
    {
        private const int _frameCount = 60;
        private readonly List<byte[]> _rawFrames = new List<byte[]>();
        private readonly List<int[]> _differenceFrames = new List<int[]>();
        private readonly int[] _differenceFrameSums = new int[_frameCount];
        private int _currentFrame;

        public NoveltyCurveNode()
        {
            for (int i = 0; i < _frameCount; i++)
            {
                _rawFrames.Add(new byte[0]);
                _differenceFrames.Add(new int[0]);
            }
            Width = 90;
            Height = 60;
            Name = "Novelty Curve";
        }

        [OutputTerminal(Direction.South)]
        public int[] NoveltyCurveOutput { get; set; }

        [InputTerminal(Direction.North)]
        public void SpectrumInputTerminalOnDataChanged(byte[] data)
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
            var differenceFrame = new int[currentFrame.Length];
            var differenceFrameSum = 0;

            for (int i = 0; i < differenceFrame.Length; i++)
            {
                differenceFrame[i] = (int)Math.Abs(currentFrame[i] - lastFrame[i]);
                differenceFrameSum += differenceFrame[i];
            }

            SetDifferenceFrame(frameNumber, differenceFrame);
            SetDifferenceFrameSum(frameNumber, differenceFrameSum);

            NoveltyCurveOutput = GetDifferenceFrame(0);
        }

        private void SetFrame(int frameIndex, byte[] frame)
        {
            _rawFrames[frameIndex + _currentFrame % _frameCount] = frame;
        }

        private byte[] GetFrame(int frameIndex)
        {
            var index = frameIndex + _currentFrame % _frameCount;
            if (index < 0)
            {
                index += _frameCount;
            }
            return _rawFrames[index];
        }

        private void SetDifferenceFrame(int frameIndex, int[] frame)
        {
            _differenceFrames[frameIndex + _currentFrame % _frameCount] = frame;
        }

        private int[] GetDifferenceFrame(int frameIndex)
        {
            return _differenceFrames[frameIndex + _currentFrame % _frameCount];
        }

        private void SetDifferenceFrameSum(int sumIndex, int sum)
        {
            _differenceFrameSums[sumIndex % _frameCount] = sum;
        }

        private int GetDifferenceFrameSum(int sumIndex)
        {
            return _differenceFrameSums[sumIndex % _frameCount];
        }
    }
}