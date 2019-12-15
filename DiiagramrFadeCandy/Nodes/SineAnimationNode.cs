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
        private float _amplitude;

        public SineAnimationNode()
        {
            Width = 60;
            Height = 60;
            Name = "Animation";
        }

        public Point[] UIPoints { get; set; }

        public Point[] HeightPoints { get; set; }

        [OutputTerminal(nameof(Output), Direction.East)]
        public float OutputFrame { get; set; }

        private double CircleQuadrents => _quadrents * HalfPI;

        [InputTerminal(nameof(Trigger), Direction.North)]
        public void Trigger(bool data)
        {
            if (data)
            {
                new Thread(() =>
                {
                    for (double d = 0.0; d <= CircleQuadrents; d += CircleQuadrents / (_frames - 1))
                    {
                        var output = (float)(_startPosition + (_amplitude * Math.Sin(d)));
                        Output(output, nameof(Output));
                        Thread.Sleep(_timeBetweenFrames);
                    }
                    Output(_startPosition, nameof(Output));
                }).Start();
            }
        }

        [InputTerminal("Frames", Direction.North)]
        public void SetFrames(int frames)
        {
            _frames = frames;
            SetQuadrents(_quadrents);
        }

        [InputTerminal("Offset", Direction.North)]
        public void SetOffset(float offset)
        {
            _startPosition = offset;
        }

        [InputTerminal("Quadrents", Direction.South)]
        public void SetQuadrents(float quadrents)
        {
            _quadrents = quadrents;
            RenderFunctionOnView();
        }

        [InputTerminal("Amplitude", Direction.West)]
        public void SetAmplitude(float amplitude)
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