using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;
using System;
using System.Threading;
using System.Windows;

namespace DiiagramrFadeCandy
{
    public enum AnimationFunction
    {
        Sine,
        Cosine
    }

    public class SineAnimationNode : Node
    {
        private const double HalfPI = Math.PI / 2.0;
        private readonly int _timeBetweenFrames = 33;
        private int _frames = 30;
        private float _startPosition = 0;
        private float _quadrents = 4;
        private float _amplitude = 1;

        public SineAnimationNode()
        {
            Width = 60;
            Height = 60;
            Name = "Sine Animation";
        }

        public Point[] UIPoints { get; set; }

        public Point[] HeightPoints { get; set; }

        [OutputTerminal(Direction.East)]
        public float OutputFrame { get; set; }

        private double CircleQuadrents => _quadrents * HalfPI;

        [InputTerminal(Direction.North)]
        public void Trigger(bool data)
        {
            if (data)
            {
                new Thread(() =>
                {
                    for (double d = 0.0; d <= CircleQuadrents; d += CircleQuadrents / (_frames - 1))
                    {
                        OutputFrame = (float)(_startPosition + (_amplitude * Math.Sin(d)));
                        Thread.Sleep(_timeBetweenFrames);
                    }
                    OutputFrame = _startPosition;
                }).Start();
            }
        }

        [InputTerminal(Direction.North)]
        public void AnimationFrames(int frames)
        {
            _frames = frames;
            AnimationQuadrents(_quadrents);
        }

        [InputTerminal(Direction.North)]
        public void StartOffset(float offset)
        {
            _startPosition = offset;
        }

        [InputTerminal(Direction.South)]
        public void AnimationQuadrents(float quadrents)
        {
            _quadrents = quadrents;
            RenderFunctionOnView();
        }

        [InputTerminal(Direction.West)]
        public void Amplitude(float amplitude)
        {
            _amplitude = amplitude;
            RenderFunctionOnView();
        }

        private void RenderFunctionOnView()
        {
            UIPoints = new Point[_frames];
            int frame = 0;
            var incrementAmount = CircleQuadrents / (_frames - 1);
            var minValue = Height;
            var minValueX = 0.0;
            for (double d = 0.0; d < CircleQuadrents + incrementAmount; d += incrementAmount)
            {
                if (frame == UIPoints.Length)
                {
                    break;
                }

                var x = frame * (Width / _frames);
                var adjustedHeight = Height - 10;
                var y = (adjustedHeight / 2) + (Math.Sin(d) * (adjustedHeight / 2));
                UIPoints[frame] = new Point(x, y);
                frame++;

                if (minValue > y)
                {
                    minValue = y;
                    minValueX = x;
                }
            }

            HeightPoints = new Point[] { new Point(minValueX, Height / 2), new Point(minValueX, minValue) };
            OnPropertyChanged(nameof(HeightPoints));
            OnPropertyChanged(nameof(UIPoints));
        }
    }
}