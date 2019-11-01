using DiiagramrAPI.Diagram.Model;
using DiiagramrAPI.Service;
using DiiagramrAPI.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media;

namespace DiiagramrAPI.Diagram
{
    public class Wire : ViewModel
    {
        private const double _dataVisualDiameter = 3;
        private const double _dataVisualRadius = _dataVisualDiameter / 2.0;
        private const int WireAnimationFrameDelay = 15;
        private const float DimWireColorAmount = -0.3f;
        private const int WireAnimationFrames = 15;
        private const int WireDataAnimationFrames = 15;
        private const int WireDataAnimationRepeatDelay = 1000;
        private bool _configuringWirePoints;
        private WirePathingAlgorithum _wirePathingAlgorithum = new WirePathingAlgorithum();
        private List<Point> _dataVisualAnimationFrames = new List<Point>();
        private List<Point[]> _wireDrawAnimationFrames = new List<Point[]>();
        private readonly bool _showDataPropagation = false;
        private bool _showingDataPropagation;
        private bool _isDataVisualAnimationFramesValid;

        public Wire(WireModel wire)
        {
            Model = wire ?? throw new ArgumentNullException(nameof(wire));
            wire.PropertyChanged += ModelPropertyChanged;
            SetWireColor();
            X1 = Model.X2;
            Y1 = Model.Y2;
            X2 = Model.X1;
            Y2 = Model.Y1;
        }

        private void DoWirePropagationAnimation()
        {
            if (_showDataPropagation && !_showingDataPropagation)
            {
                _showingDataPropagation = true;
                new Thread(() =>
                {
                    ValidateDataVisualAnimationFrames();
                    AnimateDataPropagation(WireAnimationFrameDelay);
                    Thread.Sleep(WireDataAnimationRepeatDelay);
                    _showingDataPropagation = false;
                }).Start();

            }
        }

        private void ValidateDataVisualAnimationFrames()
        {
            if (!_isDataVisualAnimationFramesValid)
            {
                GenerateDataVisualAnimationFrames(Points);
            }
        }

        /// <summary>
        /// Creates a wire that's unattched to a model.
        /// </summary>
        public Wire(Terminal startTerminal, double x1, double y1)
        {
            X2 = startTerminal.Model.X;
            Y2 = startTerminal.Model.Y;
            BannedDirectionForEnd = DirectionHelpers.OppositeDirection(startTerminal.Model.DefaultSide);
            BannedDirectionForStart = Direction.None;
            X1 = x1;
            Y1 = y1;
            _wirePathingAlgorithum.FallbackSourceTerminal = startTerminal.Model is InputTerminalModel ? startTerminal : null;
            _wirePathingAlgorithum.FallbackSinkTerminal = startTerminal.Model is OutputTerminalModel ? startTerminal : null;
            LineColorBrush = new SolidColorBrush(Colors.White);
        }

        public Brush LineColorBrush { get; set; } = Brushes.Black;
        public Point[] Points { get; set; }
        public WireModel Model { get; private set; }
        public double X1 { get; set; }
        public double X2 { get; set; }
        public double Y1 { get; set; }
        public double Y2 { get; set; }
        public double DataVisualX { get; set; }
        public double DataVisualY { get; set; }
        public bool IsDataVisualVisible { get; set; }
        public Direction BannedDirectionForStart { get; set; }
        public Direction BannedDirectionForEnd { get; set; }
        public bool DoAnimationWhenViewIsLoaded { get; set; }
        public double DataVisualDiameter { get; } = _dataVisualDiameter;

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
                    _isDataVisualAnimationFramesValid = false;
                }
            }
            base.OnPropertyChanged(propertyName);
        }

        private void ModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(Model.X2)))
            {
                X1 = Model.X2;
            }
            else if (e.PropertyName.Equals(nameof(Model.X1)))
            {
                X2 = Model.X1;
            }
            else if (e.PropertyName.Equals(nameof(Model.Y2)))
            {
                Y1 = Model.Y2;
            }
            else if (e.PropertyName.Equals(nameof(Model.Y1)))
            {
                Y2 = Model.Y1;
            }
            else if (e.PropertyName.Equals(nameof(Model.SourceTerminal)) || e.PropertyName.Equals(nameof(Model.SinkTerminal)))
            {
                Model.PropertyChanged -= ModelPropertyChanged;
            }
        }

        public void DisconnectWire()
        {
            Model.SinkTerminal.DisconnectWire(Model, Model.SourceTerminal);
        }

        protected override void OnViewLoaded()
        {
            if (DoAnimationWhenViewIsLoaded)
            {
                AnimateAndConfigureWirePoints();
            }
            else
            {
                Points = _wirePathingAlgorithum.GetWirePoints(Model, X1, Y1, X2, Y2, BannedDirectionForStart, BannedDirectionForEnd);
                _isDataVisualAnimationFramesValid = false;
            }
        }

        private void AnimateAndConfigureWirePoints()
        {
            if (!_configuringWirePoints && Model != null)
            {
                _configuringWirePoints = true;

                new Thread(() =>
                {
                    AnimateWirePointsOnUiThread(WireAnimationFrameDelay);
                    _configuringWirePoints = false;
                }).Start();
            }
        }

        private void AnimateWirePointsOnUiThread(int frameDelay)
        {
            var wirePoints = _wirePathingAlgorithum.GetWirePoints(Model, X1, Y1, X2, Y2, BannedDirectionForStart, BannedDirectionForEnd);
            // If you want the wire to draw the other way reverse wirePoints array.
            // Array.Reverse(wirePoints);

            GenerateFramesOfWiringAnimation(wirePoints);
            foreach (var frame in _wireDrawAnimationFrames)
            {
                View?.Dispatcher.Invoke(() =>
                {
                    Points = frame;
                });

                Thread.Sleep(frameDelay);
            }

            _isDataVisualAnimationFramesValid = false;
        }

        private void AnimateDataPropagation(int frameDelay)
        {
            if (_dataVisualAnimationFrames.Count == 0)
            {
                GenerateDataVisualAnimationFrames(Points);
            }
            View?.Dispatcher.Invoke(() =>
            {
                IsDataVisualVisible = true;
            });
            foreach (var dataPointAlongWire in _dataVisualAnimationFrames)
            {
                View?.Dispatcher.Invoke(() =>
                {
                    DataVisualX = dataPointAlongWire.X;
                    DataVisualY = dataPointAlongWire.Y;
                });

                Thread.Sleep(frameDelay);
            }
            View?.Dispatcher.Invoke(() =>
            {
                IsDataVisualVisible = false;
            });
        }

        private void GenerateFramesOfWiringAnimation(Point[] originalPoints)
        {
            if (originalPoints.Length == 0)
            {
                return;
            }

            _wireDrawAnimationFrames.Clear();
            var totalLength = GetLengthOfWire(originalPoints);

            for (int frameNumber = 0; frameNumber < WireAnimationFrames; frameNumber++)
            {
                var frame = new List<Point>();
                var lengthSoFar = 0.0;

                for (int j = 0; j < originalPoints.Length - 2; j++)
                {
                    frame.Add(originalPoints[j]);
                    var nextLength = Point.Subtract(originalPoints[j], originalPoints[j + 1]).Length;
                    var precentAlongAnimation = (double)frameNumber / WireAnimationFrames * (Math.PI / 2.0);
                    var targetLength = totalLength * Math.Sin(precentAlongAnimation);
                    if (lengthSoFar + nextLength > targetLength)
                    {
                        var desiredLength = targetLength - lengthSoFar;
                        var desiredPrecentOfNextLength = desiredLength / nextLength;
                        Point interpolatedPoint = GetInterpolatedPoint(originalPoints[j], originalPoints[j + 1], desiredPrecentOfNextLength);
                        frame.Add(interpolatedPoint);
                        break;
                    }
                    lengthSoFar += nextLength;
                }

                _wireDrawAnimationFrames.Add(frame.ToArray());
            }

            _wireDrawAnimationFrames.Add(originalPoints);
        }

        private void GenerateDataVisualAnimationFrames(Point[] originalPoints)
        {
            if (originalPoints.Length == 0)
            {
                return;
            }

            _dataVisualAnimationFrames.Clear();
            AddDataVisualAnimationFrame(originalPoints.First());
            var totalLength = GetLengthOfWire(originalPoints);

            for (int frameNumber = 0; frameNumber < WireAnimationFrames; frameNumber++)
            {
                var lengthSoFar = 0.0;

                for (int j = 0; j < originalPoints.Length - 1; j++)
                {
                    var nextLength = Point.Subtract(originalPoints[j], originalPoints[j + 1]).Length;
                    var precentAlongAnimation = (double)frameNumber / WireDataAnimationFrames * (Math.PI / 2.0);
                    var targetLength = totalLength * Math.Sin(precentAlongAnimation);
                    if (lengthSoFar + nextLength > targetLength)
                    {
                        var desiredLength = targetLength - lengthSoFar;
                        var desiredPrecentOfNextLength = desiredLength / nextLength;
                        Point interpolatedPoint = GetInterpolatedPoint(originalPoints[j], originalPoints[j + 1], desiredPrecentOfNextLength);
                        AddDataVisualAnimationFrame(interpolatedPoint);
                        break;
                    }
                    lengthSoFar += nextLength;
                }
            }
            AddDataVisualAnimationFrame(originalPoints.Last());
            // reverse the frames if you want the wire to draw backwards
            // _dataVisualAnimationFrames.Reverse();
            _isDataVisualAnimationFramesValid = true;
        }

        private void AddDataVisualAnimationFrame(Point p)
        {
            var pointOffsetForVisualToBeCentered = new Point(p.X - _dataVisualRadius, p.Y - _dataVisualRadius);
            _dataVisualAnimationFrames.Add(pointOffsetForVisualToBeCentered);
        }

        private static Point GetInterpolatedPoint(Point point1, Point point2, double desiredPrecentOfSegment)
        {
            var diff = Point.Subtract(point2, point1);
            var interpolatedX = point1.X + diff.X * desiredPrecentOfSegment;
            var interpolatedY = point1.Y + diff.Y * desiredPrecentOfSegment;
            var interpolatedPoint = new Point(interpolatedX, interpolatedY);
            return interpolatedPoint;
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
