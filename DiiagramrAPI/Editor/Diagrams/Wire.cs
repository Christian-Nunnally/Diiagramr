using DiiagramrAPI.Application;
using DiiagramrAPI.Editor.Interactors;
using DiiagramrAPI.Service.Application;
using DiiagramrCore;
using DiiagramrModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media;

namespace DiiagramrAPI.Editor.Diagrams
{
    public class Wire : ViewModel, IMouseEnterLeaveReaction
    {
        public static bool ShowDataPropagation = true;
        private const double _dataVisualDiameter = 6;
        private const double _dataVisualRadius = _dataVisualDiameter / 2.0;
        private const float DimWireColorAmount = -0.3f;
        private const float SuperDimWireColorAmount = -0.7f;
        private const float BrightWireColorAmount = 0.8f;
        private const int WireAnimationFrameDelay = 20;
        private const int WireAnimationFrames = 15;
        private const int WireDataAnimationFrames = 30;
        private const int WireDataAnimationRepeatDelay = 500;
        private readonly List<Point[]> _wireDrawAnimationFrames = new List<Point[]>();
        private readonly WirePathingAlgorithum _wirePathingAlgorithum;
        private readonly List<Point> _dataVisualAnimationFrames = new List<Point>();
        private readonly Brush BrokenWireBrush = new SolidColorBrush(Color.FromRgb(90, 9, 9));
        private bool _configuringWirePoints;
        private bool _doDataVisualAnimationFramesNeedToBeRefreshed;
        private bool _isMouseOver;

        private int _dataChangedCount;
        private bool _repeatDataPropagation;
        private BackgroundTask _wirePropagationAnimationTask;

        public Wire(WireModel wire)
        {
            _wirePathingAlgorithum = new WirePathingAlgorithum(this);
            WireModel = wire ?? throw new ArgumentNullException(nameof(wire));
            wire.PropertyChanged += ModelPropertyChanged;
            UpdateWireColor();
        }

        public Wire(Terminal startTerminal, double x1, double y1)
        {
            WireModel = new WireModel()
            {
                X1 = x1,
                X2 = startTerminal?.Model.X ?? throw new ArgumentNullException(nameof(startTerminal)),
                Y1 = y1,
                Y2 = startTerminal.Model.Y,
            };
            BannedDirectionForEnd = startTerminal.Model.DefaultSide.Opposite();
            BannedDirectionForStart = Direction.None;
            _wirePathingAlgorithum = new WirePathingAlgorithum(this)
            {
                FallbackSourceTerminal = startTerminal.Model is InputTerminalModel ? startTerminal : null,
                FallbackSinkTerminal = startTerminal.Model is OutputTerminalModel ? startTerminal : null
            };
            LineColorBrush = new SolidColorBrush(Colors.White);
        }

        public string WirePropagationVisualNumberString { get; set; }
        public Direction BannedDirectionForEnd { get; set; }

        public Direction BannedDirectionForStart { get; set; }

        public DoubleCollection StrokeDashArray { get; set; }

        public double DataVisualDiameter { get; } = _dataVisualDiameter;

        public double DataVisualX { get; set; }

        public double DataVisualY { get; set; }

        public bool IsBroken => WireModel?.IsBroken ?? true;

        public bool DoAnimationWhenViewIsLoaded { get; set; }

        public bool IsDataVisualVisible { get; set; }

        public Brush LineColorBrush { get; set; } = Brushes.Black;
        public Brush PropagationVisualColorBrush { get; set; } = Brushes.Black;

        public WireModel WireModel { get; private set; }

        public IList<Point> Points { get; private set; }

        public double X1 => WireModel.X1;

        public double X2 => WireModel.X2;

        public double Y1 => WireModel.Y1;

        public double Y2 => WireModel.Y2;

        private bool CanStartPropagationAnimationTask => ShowDataPropagation && !IsPropagationAnimationTaskRunning;

        private bool IsPropagationAnimationTaskRunning => _wirePropagationAnimationTask != null && _wirePropagationAnimationTask.IsRunning;

        public void DisconnectWire()
        {
            WireModel.SinkTerminal.DisconnectWire(WireModel, WireModel.SourceTerminal);
        }

        public void MouseEntered()
        {
            _isMouseOver = true;
            UpdateWireColor();
        }

        public void MouseLeft()
        {
            _isMouseOver = false;
            UpdateWireColor();
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            if (propertyName == nameof(X1)
             || propertyName == nameof(X2)
             || propertyName == nameof(Y1)
             || propertyName == nameof(Y2))
            {
                if (View != null)
                {
                    Points = _wirePathingAlgorithum.GetWirePoints(X2, Y2, X1, Y1, BannedDirectionForStart, BannedDirectionForEnd);
                    _doDataVisualAnimationFramesNeedToBeRefreshed = false;
                }
            }

            base.OnPropertyChanged(propertyName);
        }

        protected override void OnViewLoaded()
        {
            if (DoAnimationWhenViewIsLoaded)
            {
                AnimateAndConfigureWirePoints();
            }
            else
            {
                Points = _wirePathingAlgorithum.GetWirePoints(X2, Y2, X1, Y1, BannedDirectionForStart, BannedDirectionForEnd);
                _doDataVisualAnimationFramesNeedToBeRefreshed = false;
            }
        }

        private static Point GetInterpolatedPoint(Point point1, Point point2, double desiredPrecentOfSegment)
        {
            var diff = Point.Subtract(point2, point1);
            var interpolatedX = point1.X + diff.X * desiredPrecentOfSegment;
            var interpolatedY = point1.Y + diff.Y * desiredPrecentOfSegment;
            var interpolatedPoint = new Point(interpolatedX, interpolatedY);
            return interpolatedPoint;
        }

        private static SolidColorBrush GetBrushFromColor(System.Drawing.Color color)
        {
            return new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
        }

        private void AddDataVisualAnimationFrame(Point p)
        {
            var pointOffsetForVisualToBeCentered = new Point(p.X - _dataVisualRadius, p.Y - _dataVisualRadius);
            _dataVisualAnimationFrames.Add(pointOffsetForVisualToBeCentered);
        }

        private void AnimateAndConfigureWirePoints()
        {
            if (!_configuringWirePoints && WireModel != null)
            {
                _configuringWirePoints = true;

                new Thread(() =>
                {
                    AnimateWirePointsOnUiThread(WireAnimationFrameDelay);
                    _configuringWirePoints = false;
                }).Start();
            }
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

        private void AnimateWirePointsOnUiThread(int frameDelay)
        {
            var wirePoints = _wirePathingAlgorithum.GetWirePoints(X2, Y2, X1, Y1, BannedDirectionForStart, BannedDirectionForEnd);

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

            _doDataVisualAnimationFramesNeedToBeRefreshed = false;
        }

        private void TryStartPropagationAnimationTask()
        {
            if (CanStartPropagationAnimationTask)
            {
                _wirePropagationAnimationTask = BackgroundTaskManager.Instance.CreateBackgroundTask(DoWirePropagationAnimation);
                WirePropagationVisualNumberString = _dataChangedCount > 1 ? _dataChangedCount.ToString() : string.Empty;
                _dataChangedCount = 0;
                _wirePropagationAnimationTask.Start();
            }
        }

        private void DoWirePropagationAnimation()
        {
            _repeatDataPropagation = false;
            ValidateDataVisualAnimationFrames();
            AnimateDataPropagation(WireAnimationFrameDelay);
            Thread.Sleep(WireDataAnimationRepeatDelay);
            if (_repeatDataPropagation)
            {
                DoWirePropagationAnimation();
            }
        }

        private void GenerateDataVisualAnimationFrames(IList<Point> originalPoints)
        {
            if (originalPoints.Count == 0)
            {
                return;
            }

            _dataVisualAnimationFrames.Clear();
            AddDataVisualAnimationFrame(originalPoints.First());
            var totalLength = GetLengthOfWire(originalPoints);

            for (int frameNumber = 0; frameNumber < WireDataAnimationFrames; frameNumber++)
            {
                var lengthSoFar = 0.0;

                for (int j = 0; j < originalPoints.Count - 1; j++)
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
            _dataVisualAnimationFrames.Reverse();
            _doDataVisualAnimationFramesNeedToBeRefreshed = true;
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

        private double GetLengthOfWire(IList<Point> wirePoints)
        {
            var length = 0.0;
            for (int i = 0; i < wirePoints.Count - 2; i++)
            {
                length += Point.Subtract(wirePoints[i], wirePoints[i + 1]).Length;
            }
            return length;
        }

        private void ModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Data":
                    AnimateWirePropagationIfNecessary();
                    break;

                case nameof(WireModel.SourceTerminal):
                case nameof(WireModel.SinkTerminal):
                    WireModel.PropertyChanged -= ModelPropertyChanged;
                    break;

                case nameof(WireModel.IsBroken):
                    VisuallyBreakWire();
                    NotifyOfPropertyChange(e.PropertyName);
                    break;

                case nameof(WireModel.X1):
                case nameof(WireModel.X2):
                case nameof(WireModel.Y1):
                case nameof(WireModel.Y2):
                    NotifyOfPropertyChange(e.PropertyName);
                    break;
            }
        }

        private void AnimateWirePropagationIfNecessary()
        {
            _dataChangedCount++;
            if (!_repeatDataPropagation)
            {
                TryStartPropagationAnimationTask();
            }
            else if (_wirePropagationAnimationTask == null || _wirePropagationAnimationTask.IsRunning)
            {
                _repeatDataPropagation = true;
            }
        }

        private void VisuallyBreakWire()
        {
            StrokeDashArray = new DoubleCollection(new[] { 1.0, 1.0 });
            UpdateWireColor();
        }

        private void UpdateWireColor()
        {
            if (WireModel.SourceTerminal == null || WireModel.SinkTerminal == null)
            {
                return;
            }

            if (IsBroken)
            {
                LineColorBrush = BrokenWireBrush;
                return;
            }

            var terminalToGetColorFrom = WireModel.SourceTerminal.Type == typeof(object)
                            ? WireModel.SinkTerminal
                            : WireModel.SourceTerminal;
            var typeToGetColorOf = terminalToGetColorFrom.CurrentType;
            var color = TypeColorProvider.Instance.GetColorForType(typeToGetColorOf);
            var finalColor = _isMouseOver
                ? CoreUilities.ChangeColorBrightness(color, SuperDimWireColorAmount)
                : CoreUilities.ChangeColorBrightness(color, DimWireColorAmount);
            LineColorBrush = GetBrushFromColor(finalColor);
            PropagationVisualColorBrush = GetBrushFromColor(CoreUilities.ChangeColorBrightness(color, BrightWireColorAmount));
        }

        private void ValidateDataVisualAnimationFrames()
        {
            if (!_doDataVisualAnimationFramesNeedToBeRefreshed)
            {
                GenerateDataVisualAnimationFrames(Points ?? new List<Point>());
            }
        }
    }
}