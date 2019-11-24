using DiiagramrAPI.Diagram;
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

    public class SineAnimationNodeViewModel : Node
    {
        private const double HalfPI = Math.PI / 2.0;
        private int _frames = 30;
        private readonly int _timeBetweenFrames = 33;
        private float _startPosition = 0;
        private float _quadrents = 4;
        private AnimationFunction _function = AnimationFunction.Sine;

        private double CircleQuadrents => _quadrents * HalfPI;
        public Point[] UIPoints { get; set; }
        public Point[] HeightPoints { get; set; }
        public TypedTerminal<bool> TriggerTerminal { get; private set; }
        public TypedTerminal<float> AmplitudeTerminal { get; private set; }
        public TypedTerminal<float> ValueTerminal { get; private set; }
        public TypedTerminal<float> OffsetTerminal { get; private set; }
        public TypedTerminal<int> FramesTerminal { get; private set; }
        public TypedTerminal<float> QuadrentsTerminal { get; private set; }
        public TypedTerminal<AnimationFunction> FunctionTerminal { get; private set; }

        protected override void SetupNode(NodeSetup setup)
        {
            setup.NodeSize(60, 60);
            setup.NodeName("Animation");

            TriggerTerminal = setup.InputTerminal<bool>("Trigger", Direction.North);
            TriggerTerminal.DataChanged += TriggerTerminalDataChanged;

            AmplitudeTerminal = setup.InputTerminal<float>("Amplitude", Direction.West);
            AmplitudeTerminal.Data = 1;

            ValueTerminal = setup.OutputTerminal<float>("Value", Direction.East);
            ValueTerminal.Data = _startPosition;

            OffsetTerminal = setup.InputTerminal<float>("Offset", Direction.West);
            OffsetTerminal.Data = _startPosition;
            OffsetTerminal.DataChanged += OffsetTerminalDataChanged;

            FramesTerminal = setup.InputTerminal<int>("Frames", Direction.North);
            FramesTerminal.Data = _frames;
            FramesTerminal.DataChanged += FramesTerminalDataChanged;

            QuadrentsTerminal = setup.InputTerminal<float>("Quadrents", Direction.South);
            QuadrentsTerminal.Data = _quadrents;
            QuadrentsTerminal.DataChanged += QuadrentsTerminalDataChanged;
            QuadrentsTerminalDataChanged(_quadrents);

            // FunctionTerminal = setup.InputTerminal<AnimationFunction>("Function", Direction.South);
            // FunctionTerminal.Data = _function;
            // FunctionTerminal.DataChanged += FunctionTerminalDataChanged;
        }

        private void FunctionTerminalDataChanged(AnimationFunction data)
        {
            _function = data;
            RenderFunctionOnView();
        }

        private void QuadrentsTerminalDataChanged(float quadrents)
        {
            _quadrents = quadrents;
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

        private void FramesTerminalDataChanged(int frames)
        {
            _frames = frames;
            QuadrentsTerminalDataChanged(_quadrents);
        }

        private void OffsetTerminalDataChanged(float offset)
        {
            _startPosition = offset;
        }

        private void TriggerTerminalDataChanged(bool data)
        {
            if (data)
            {
                new Thread(() =>
                {
                    for (double d = 0.0; d <= CircleQuadrents; d += CircleQuadrents / (_frames - 1))
                    {
                        ValueTerminal.Data = (float)(_startPosition + (AmplitudeTerminal.Data * Math.Sin(d)));
                        Thread.Sleep(_timeBetweenFrames);
                    }
                    ValueTerminal.Data = _startPosition;
                }).Start();
            }
        }
    }
}
