using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;

namespace DiiagramrFadeCandy
{
    [Help("Node that generates customizeable wave signals over time. The signal can be triggered in bursts or continuously. The amplitute, sample rate, and period can all also be configured.")]
    public class WaveGeneratorNode : Node
    {
        private const float HalfPI = (float)Math.PI / 2.0f;
        private readonly int _timeBetweenFrames = 15;
        private readonly Dictionary<WaveType, Func<float, float>> _waveGenerationFunctions = new Dictionary<WaveType, Func<float, float>>();
        private readonly Random _random = new Random();
        private int _frames = 30;
        private float _startPosition = .5f;
        private float _quadrents = 4;
        private float _amplitude = .5f;
        private WaveType waveType;

        public WaveGeneratorNode()
        {
            Width = 60;
            Height = 60;
            Name = "Wave Generator";
            _waveGenerationFunctions.Add(WaveType.Sine, x => (float)Math.Sin(x));
            _waveGenerationFunctions.Add(WaveType.Cosine, x => (float)Math.Cos(x));
            _waveGenerationFunctions.Add(WaveType.Triangle, x => 1 - Math.Abs((x % (2 * HalfPI)) - HalfPI));
            _waveGenerationFunctions.Add(WaveType.Square, x => (x % Math.PI) < HalfPI ? 1 : 0);
            _waveGenerationFunctions.Add(WaveType.Saw, x => (x % HalfPI) / HalfPI);
            _waveGenerationFunctions.Add(WaveType.Noise, x => (float)_random.NextDouble());
            RenderFunctionOnView();
        }

        public Point[] UIPoints { get; set; }

        public Point[] HeightPoints { get; set; }

        [NodeSetting]
        [OutputTerminal(Direction.South)]
        [Help("The current value of the generated wave signal.")]
        public float WaveSignal { get; set; }

        [NodeSetting]
        [InputTerminal(Direction.East)]
        [Help("A constant value to offset the generated wave by.")]
        public float StartOffset
        {
            get => _startPosition;
            set
            {
                _startPosition = value;
                RenderFunctionOnView();
            }
        }

        [NodeSetting]
        [InputTerminal(Direction.North)]
        [Help("If true, the generated wave will repeat continuously. If false, the generated wave will only continue until a certain number of quadrants have been generated.")]
        public bool Repeat { get; set; }

        [NodeSetting]
        [InputTerminal(Direction.North)]
        [Help("When set to true, begins generating the wave signal.")]
        public bool Trigger
        {
            get => true;
            set => RunAnimation();
        }

        [NodeSetting]
        [InputTerminal(Direction.West)]
        [Help("The number of wave samples to generate each time the node is triggered.")]
        public int SignalSamples
        {
            get => _frames;
            set
            {
                _frames = value;
                RenderFunctionOnView();
            }
        }

        [NodeSetting]
        [InputTerminal(Direction.West)]
        [Help("The number of quadrents to generate each time the node is triggered.")]
        public float WaveQuadrents
        {
            get => _quadrents;
            set
            {
                _quadrents = value;
                RenderFunctionOnView();
            }
        }

        [NodeSetting]
        [InputTerminal(Direction.East)]
        [Help("Sets the amplitude of the wave signal.")]
        public float Amplitude
        {
            get => _amplitude;
            set
            {
                _amplitude = value;
                RenderFunctionOnView();
            }
        }

        [NodeSetting]
        [InputTerminal(Direction.North)]
        [Help("Sets the function used to generate the wave signal.")]
        public WaveType WaveType
        {
            get => waveType;
            set
            {
                waveType = value;
                RenderFunctionOnView();
            }
        }

        private float CircleQuadrents => _quadrents * HalfPI;

        private void RunAnimation()
        {
            new Thread(() =>
            {
                do
                {
                    for (float d = 0.0f; d <= CircleQuadrents; d += CircleQuadrents / (_frames - 1))
                    {
                        WaveSignal = _startPosition + (_amplitude * _waveGenerationFunctions[WaveType](d));
                        Thread.Sleep(_timeBetweenFrames);
                    }
                    WaveSignal = _startPosition;
                    Thread.Sleep(_timeBetweenFrames);
                } while (Repeat);
            }).Start();
        }

        private void RenderFunctionOnView()
        {
            UIPoints = new Point[_frames];
            int frame = 0;
            var incrementAmount = CircleQuadrents / (_frames - 1);
            var minValue = Height;
            var minValueX = 0.0;
            for (float d = 0.0f; d <= CircleQuadrents + incrementAmount; d += incrementAmount)
            {
                if (frame == UIPoints.Length)
                {
                    break;
                }

                var x = (float)(frame * (Width / _frames));
                var adjustedHeight = Height - 10;
                var y = (Height / 2) - (_waveGenerationFunctions[WaveType](d) * (adjustedHeight / 2));
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