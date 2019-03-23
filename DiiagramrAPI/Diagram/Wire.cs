using DiiagramrAPI.Diagram.Model;
using DiiagramrAPI.Service;
using DiiagramrAPI.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace DiiagramrAPI.Diagram
{
    public class Wire : ViewModel
    {
        private const int WireAnimationFrameDelay = 15;
        private const float DimWireColorAmount = -0.3f;
        private const int WireAnimationFrames = 15;
        private bool _configuringWirePoints;
        private WirePathingAlgorithum _wirePathingAlgorithum = new WirePathingAlgorithum();

        public Wire(WireModel wire)
        {
            Model = wire ?? throw new ArgumentNullException(nameof(wire));
            wire.PropertyChanged += ModelPropertyChanged;
            IsAttachedToModel = true;
            SetWireColor();
            X1 = Model.X1 + Diagram.NodeBorderWidth;
            Y1 = Model.Y1 + Diagram.NodeBorderWidth;
            X2 = Model.X2 + Diagram.NodeBorderWidth;
            Y2 = Model.Y2 + Diagram.NodeBorderWidth;
        }

        /// <summary>
        /// Creates a wire that's unattched to a model.
        /// </summary>
        public Wire(Terminal startTerminal, double x1, double y1)
        {
            X2 = startTerminal.Model.X + Terminal.TerminalDiameter / 2;
            Y2 = startTerminal.Model.Y + Terminal.TerminalDiameter / 2;
            BannedDirectionForEnd = DirectionHelpers.OppositeDirection(startTerminal.Model.Direction);
            BannedDirectionForStart = Direction.None;
            X1 = x1;
            Y1 = y1;
            _wirePathingAlgorithum.FallbackSourceTerminal = startTerminal.Model.Kind == TerminalKind.Input ? startTerminal : null;
            _wirePathingAlgorithum.FallbackSinkTerminal = startTerminal.Model.Kind == TerminalKind.Output ? startTerminal : null;
            LineColorBrush = new SolidColorBrush(Colors.White);
        }

        public Brush LineColorBrush { get; set; } = Brushes.Black;
        public Point[] Points { get; set; }
        public WireModel Model { get; private set; }
        public double X1 { get; set; }
        public double X2 { get; set; }
        public double Y1 { get; set; }
        public double Y2 { get; set; }
        public Direction BannedDirectionForStart { get; set; }
        public Direction BannedDirectionForEnd { get; set; }
        public bool IsAttachedToModel { get; set; } = false;
        public bool DoAnimationWhenViewIsLoaded { get; set; } = true;

        private void SetWireColor()
        {
            var terminalToGetColorFrom = Model.SinkTerminal.Type == typeof(object)
                            ? Model.SourceTerminal
                            : Model.SinkTerminal;
            var typeToGetColorOf = terminalToGetColorFrom.Type;
            var color = TypeColorProvider.Instance.GetColorForType(typeToGetColorOf);
            var darkenedColor = CoreUilities.ChangeColorBrightness(color, DimWireColorAmount);
            LineColorBrush = new SolidColorBrush(darkenedColor);
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            if (propertyName.Equals(nameof(X1))
             || propertyName.Equals(nameof(X2))
             || propertyName.Equals(nameof(Y1))
             || propertyName.Equals(nameof(Y2)))
            {
                if (View != null)
                {
                    Points = _wirePathingAlgorithum.GetWirePoints(Model, X1, Y1, X2, Y2, BannedDirectionForStart, BannedDirectionForEnd);
                }
            }
            base.OnPropertyChanged(propertyName);
        }

        private void ModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(Model.X1)))
            {
                X1 = Model.X1 + Diagram.NodeBorderWidth;
            }
            else if (e.PropertyName.Equals(nameof(Model.X2)))
            {
                X2 = Model.X2 + Diagram.NodeBorderWidth;
            }
            else if (e.PropertyName.Equals(nameof(Model.Y1)))
            {
                Y1 = Model.Y1 + Diagram.NodeBorderWidth;
            }
            else if (e.PropertyName.Equals(nameof(Model.Y2)))
            {
                Y2 = Model.Y2 + Diagram.NodeBorderWidth;
            }
            else if (e.PropertyName.Equals(nameof(Model.SourceTerminal)) || e.PropertyName.Equals(nameof(Model.SinkTerminal)))
            {
                Model.PropertyChanged -= ModelPropertyChanged;
            }
        }

        public void DisconnectWire()
        {
            Model?.DisconnectWire();
        }

        public void WireMouseDown(object sender, MouseEventArgs e)
        {
            DisconnectWire();
        }

        protected override void OnViewLoaded()
        {
            if (DoAnimationWhenViewIsLoaded)
            {
                AnimateAndConfigureWirePoints();
            }
        }

        private void AnimateAndConfigureWirePoints()
        {
            if (_configuringWirePoints || Model == null)
            {
                return;
            }

            _configuringWirePoints = true;

            new Thread(() =>
            {
                AnimateWirePointsOnUiThread(WireAnimationFrameDelay);
                _configuringWirePoints = false;
            }).Start();
        }

        private void AnimateWirePointsOnUiThread(int frameDelay)
        {
            var wirePoints = _wirePathingAlgorithum.GetWirePoints(Model, X1, Y1, X2, Y2, BannedDirectionForStart, BannedDirectionForEnd);
            if (Model.UserWiredFromInput)
            {
                Array.Reverse(wirePoints);
            }

            var animation = GenerateFramesOfWiringAnimation(wirePoints);
            foreach (var frame in animation)
            {
                View?.Dispatcher.Invoke(() =>
                {
                    Points = frame;
                });

                Thread.Sleep(frameDelay);
            }
        }

        private List<Point[]> GenerateFramesOfWiringAnimation(Point[] originalPoints)
        {
            if (originalPoints.Length == 0)
            {
                return new List<Point[]>();
            }

            var animation = new List<Point[]>();
            var totalLength = GetLengthOfWire(originalPoints);

            for (int frameNumber = 0; frameNumber < WireAnimationFrames; frameNumber++)
            {
                var frame = new List<Point> { originalPoints[0] };
                var lengthSoFar = 0.0;

                for (int j = 0; j < originalPoints.Length - 2; j++)
                {
                    frame.Add(originalPoints[j]);
                    var nextLength = Point.Subtract(originalPoints[j], originalPoints[j + 1]).Length;
                    var targetLength = totalLength * Math.Sin((double)frameNumber / WireAnimationFrames * (Math.PI / 2.0));
                    if (lengthSoFar + nextLength > targetLength)
                    {
                        var diff_X = originalPoints[j + 1].X - originalPoints[j].X;
                        var diff_Y = originalPoints[j + 1].Y - originalPoints[j].Y;
                        var differenceLength = targetLength - lengthSoFar;
                        var ratioOfCurrentLengthToDisplay = differenceLength / nextLength;
                        var interpolatedX = originalPoints[j].X + diff_X * ratioOfCurrentLengthToDisplay;
                        var interpolatedY = originalPoints[j].Y + diff_Y * ratioOfCurrentLengthToDisplay;
                        var interpolatedPoint = new Point(interpolatedX, interpolatedY);
                        frame.Add(interpolatedPoint);
                        break;
                    }
                    lengthSoFar += nextLength;
                }

                animation.Add(frame.ToArray());
            }

            animation.Add(originalPoints);
            return animation;
        }

        private double GetLengthOfWire(Point[] wirePoints)
        {
            var length = 0.0;
            for (int i = 0; i < wirePoints.Length - 2; i++)
            {
                length += Point.Subtract(wirePoints[i], wirePoints[i + 1]).Length;
            }
            return length;
        }
    }
}
