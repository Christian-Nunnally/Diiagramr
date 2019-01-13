using DiiagramrAPI.Model;
using DiiagramrAPI.PluginNodeApi;
using DiiagramrAPI.Service;
using DiiagramrAPI.ViewModel.ProjectScreen.Diagram;
using Stylet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace DiiagramrAPI.ViewModel.Diagram
{
    public class WireViewModel : Screen
    {
        private const double WireDistanceOutOfTerminal = 25.0;
        private const double WireEdgeIndexSpacing = 0.0;
        private const int WireAnimationFrameDelay = 15;
        private bool _configuringWirePoints;

        public ColorTheme ColorTheme
        {
            private get => _colorTheme;

            set
            {
                _colorTheme = value;
                if (ColorTheme != null)
                {
                    LineColorBrush = new SolidColorBrush(ColorTheme.GetWireColorForType(WireModel.SinkTerminal.Type));
                }
            }
        }

        public Brush LineColorBrush { get; set; } = Brushes.Black;
        private ColorTheme _colorTheme;

        private double UpUTurnLengthSink => WireModel.SinkTerminal.TerminalUpWireMinimumLength;
        private double DownUTurnLengthSink => WireModel.SinkTerminal.TerminalDownWireMinimumLength;
        private double LeftUTurnLengthSink => WireModel.SinkTerminal.TerminalLeftWireMinimumLength;
        private double RightUTurnLengthSink => WireModel.SinkTerminal.TerminalRightWireMinimumLength;

        private double UpUTurnLengthSource => WireModel.SourceTerminal.TerminalUpWireMinimumLength;
        private double DownUTurnLengthSource => WireModel.SourceTerminal.TerminalDownWireMinimumLength;
        private double LeftUTurnLengthSource => WireModel.SourceTerminal.TerminalLeftWireMinimumLength;
        private double RightUTurnLengthSource => WireModel.SourceTerminal.TerminalRightWireMinimumLength;

        public WireViewModel(WireModel wire)
        {
            WireModel = wire ?? throw new ArgumentNullException(nameof(wire));
            wire.PropertyChanged += WireOnPropertyChanged;

            X1 = wire.X1 + DiagramViewModel.NodeBorderWidth;
            X2 = wire.X2 + DiagramViewModel.NodeBorderWidth;
            Y1 = wire.Y1 + DiagramViewModel.NodeBorderWidth;
            Y2 = wire.Y2 + DiagramViewModel.NodeBorderWidth;
        }

        public Point[] Points { get; set; }

        public double X1 { get; private set; }
        public double X2 { get; private set; }
        public double Y1 { get; private set; }
        public double Y2 { get; private set; }

        public WireModel WireModel { get; private set; }

        private void WireOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(WireModel.X1)))
            {
                X1 = WireModel.X1 + DiagramViewModel.NodeBorderWidth;
            }

            if (e.PropertyName.Equals(nameof(WireModel.X2)))
            {
                X2 = WireModel.X2 + DiagramViewModel.NodeBorderWidth;
            }

            if (e.PropertyName.Equals(nameof(WireModel.Y1)))
            {
                Y1 = WireModel.Y1 + DiagramViewModel.NodeBorderWidth;
            }

            if (e.PropertyName.Equals(nameof(WireModel.Y2)))
            {
                Y2 = WireModel.Y2 + DiagramViewModel.NodeBorderWidth;
            }

            if (e.PropertyName.Equals(nameof(WireModel.SourceTerminal)) || e.PropertyName.Equals(nameof(WireModel.SinkTerminal)))
            {
                WireModel.PropertyChanged -= WireOnPropertyChanged;
                return;
            }

            if (View != null)
            {
                Points = GetWirePoints();
            }
        }

        private void AnimateAndConfigureWirePoints()
        {
            if (_configuringWirePoints)
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

        protected override void OnViewLoaded()
        {
            AnimateAndConfigureWirePoints();
        }

        private void AnimateWirePointsOnUiThread(int frameDelay)
        {
            var wirePoints = GetWirePoints();
            if (WireModel.UserWiredFromInput)
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

            const int frames = 15;
            var animation = new List<Point[]>();
            var totalLength = GetLengthOfWire(originalPoints);

            for (int frameNumber = 0; frameNumber < frames; frameNumber++)
            {
                var frame = new List<Point> { originalPoints[0] };
                var lengthSoFar = 0.0;

                for (int j = 0; j < originalPoints.Length - 2; j++)
                {
                    frame.Add(originalPoints[j]);
                    var nextLength = Point.Subtract(originalPoints[j], originalPoints[j + 1]).Length;
                    var targetLength = totalLength * Math.Sin((double)frameNumber / frames * (Math.PI / 2.0));
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

        private Point[] GetWirePoints()
        {
            var start = new Point(X1, Y1);
            var end = new Point(X2, Y2);
            var startEdgeIndexExtensionLength = WireModel.SinkTerminal.EdgeIndex * WireEdgeIndexSpacing;
            var endEdgeIndexExtensionLength = WireModel.SourceTerminal.EdgeIndex * WireEdgeIndexSpacing;
            var stubStart = TranslatePointInDirection(start, WireModel.SinkTerminal.Direction, WireDistanceOutOfTerminal + startEdgeIndexExtensionLength);
            var stubEnd = TranslatePointInDirection(end, WireModel.SourceTerminal.Direction, WireDistanceOutOfTerminal + endEdgeIndexExtensionLength);

            var backwardPoints = new List<Point>
            {
                end
            };
            var bannedDirectionForStart = OppositeDirection(WireModel.SinkTerminal.Direction);
            var bannedDirectionForEnd = OppositeDirection(WireModel.SourceTerminal.Direction);

            _uTurned = false;
            WireTwoPoints(stubEnd, stubStart, bannedDirectionForEnd, bannedDirectionForStart, backwardPoints, 2, true);
            if (_uTurned)
            {
                bannedDirectionForEnd = GetBannedDirectionFromPoints(backwardPoints[1], backwardPoints[2]);
                stubEnd = backwardPoints[2];
            }

            var points = new List<Point>
            {
                start
            };
            WireTwoPoints(stubStart, stubEnd, bannedDirectionForStart, bannedDirectionForEnd, points);

            if (_uTurned)
            {
                points.Add(backwardPoints[1]);
            }

            points.Add(end);

            return points.ToArray();
        }

        private static Direction GetBannedDirectionFromPoints(Point start, Point end)
        {
            if (Math.Abs(start.X - end.X) > Math.Abs(start.Y - end.Y))
            {
                return start.X - end.X > 0 ? Direction.East : Direction.West;
            }
            else
            {
                return start.Y - end.Y > 0 ? Direction.South : Direction.North;
            }
        }

        private static Point TranslatePointInDirection(Point p, Direction direction, double amount)
        {
            if (direction == Direction.North)
            {
                return new Point(p.X, p.Y - amount);
            }

            if (direction == Direction.South)
            {
                return new Point(p.X, p.Y + amount);
            }

            return direction == Direction.East ? new Point(p.X + amount, p.Y) : new Point(p.X - amount, p.Y);
        }

        public void WireMouseDown(object sender, MouseEventArgs e)
        {
            DisconnectWire();
        }

        public void DisconnectWire()
        {
            WireModel.DisconnectWire();
        }

        private IList<Point> WireTwoPoints(Point start, Point end, Direction bannedDirectionForStart, Direction bannedDirectionForEnd, IList<Point> pointsSoFar)
        {
            return WireTwoPoints(start, end, bannedDirectionForStart, pointsSoFar, bannedDirectionForEnd, 0, -1, false);
        }

        private IList<Point> WireTwoPoints(Point start, Point end, Direction bannedDirectionForStart, Direction bannedDirectionForEnd, IList<Point> pointsSoFar, int maxNumberOfPoints, bool fromSourceTerminal)
        {
            return WireTwoPoints(start, end, bannedDirectionForStart, pointsSoFar, bannedDirectionForEnd, 0, maxNumberOfPoints, fromSourceTerminal);
        }

        private IList<Point> WireTwoPoints(Point start, Point end, Direction bannedDirectionForStart, IList<Point> pointsSoFar, Direction bannedDirectionForEnd, int uturnCount, int maxNumberOfPoints, bool fromSourceTerminal)
        {
            pointsSoFar.Add(start);

            if (Math.Abs(start.Y - end.Y) < 0.5 || Math.Abs(start.X - end.X) < 0.5 || maxNumberOfPoints == pointsSoFar.Count - 1)
            {
                pointsSoFar.Add(end);
                return pointsSoFar;
            }

            switch (bannedDirectionForStart)
            {
                case Direction.South:
                    // The end is above the start
                    if (start.Y >= end.Y)
                    {
                        if (bannedDirectionForEnd == Direction.East)
                        {
                            if (start.X > end.X)
                            {
                                return WireHorizontiallyTowardsEnd(start, end, bannedDirectionForEnd, pointsSoFar, uturnCount, maxNumberOfPoints, fromSourceTerminal);
                            }
                            else
                            {
                                return WireVerticallyTowardsEnd(start, end, bannedDirectionForEnd, pointsSoFar, uturnCount, maxNumberOfPoints, fromSourceTerminal);
                            }
                        }

                        if (bannedDirectionForEnd == Direction.West)
                        {
                            if (start.X > end.X)
                            {
                                return WireVerticallyTowardsEnd(start, end, bannedDirectionForEnd, pointsSoFar, uturnCount, maxNumberOfPoints, fromSourceTerminal);
                            }
                            else
                            {
                                return WireHorizontiallyTowardsEnd(start, end, bannedDirectionForEnd, pointsSoFar, uturnCount, maxNumberOfPoints, fromSourceTerminal);
                            }
                        }

                        return WireVerticallyTowardsEnd(start, end, bannedDirectionForEnd, pointsSoFar, uturnCount, maxNumberOfPoints, fromSourceTerminal);
                    }
                    break;

                case Direction.West:
                    // The end is to the left of the start
                    if (start.X <= end.X)
                    {
                        if (bannedDirectionForEnd == Direction.North)
                        {
                            if (start.Y > end.Y)
                            {
                                return WireHorizontiallyTowardsEnd(start, end, bannedDirectionForEnd, pointsSoFar, uturnCount, maxNumberOfPoints, fromSourceTerminal);
                            }
                            else
                            {
                                return WireVerticallyTowardsEnd(start, end, bannedDirectionForEnd, pointsSoFar, uturnCount, maxNumberOfPoints, fromSourceTerminal);
                            }
                        }

                        if (bannedDirectionForEnd == Direction.South)
                        {
                            if (start.Y > end.Y)
                            {
                                return WireVerticallyTowardsEnd(start, end, bannedDirectionForEnd, pointsSoFar, uturnCount, maxNumberOfPoints, fromSourceTerminal);
                            }
                            else
                            {
                                return WireHorizontiallyTowardsEnd(start, end, bannedDirectionForEnd, pointsSoFar, uturnCount, maxNumberOfPoints, fromSourceTerminal);
                            }
                        }

                        return WireHorizontiallyTowardsEnd(start, end, bannedDirectionForEnd, pointsSoFar, uturnCount, maxNumberOfPoints, fromSourceTerminal);
                    }
                    break;

                case Direction.North:
                    // The end is below the start
                    if (start.Y <= end.Y)
                    {
                        if (bannedDirectionForEnd == Direction.East)
                        {
                            if (start.X > end.X)
                            {
                                return WireHorizontiallyTowardsEnd(start, end, bannedDirectionForEnd, pointsSoFar, uturnCount, maxNumberOfPoints, fromSourceTerminal);
                            }
                            else
                            {
                                return WireVerticallyTowardsEnd(start, end, bannedDirectionForEnd, pointsSoFar, uturnCount, maxNumberOfPoints, fromSourceTerminal);
                            }
                        }

                        if (bannedDirectionForEnd == Direction.West)
                        {
                            if (start.X > end.X)
                            {
                                return WireVerticallyTowardsEnd(start, end, bannedDirectionForEnd, pointsSoFar, uturnCount, maxNumberOfPoints, fromSourceTerminal);
                            }
                            else
                            {
                                return WireHorizontiallyTowardsEnd(start, end, bannedDirectionForEnd, pointsSoFar, uturnCount, maxNumberOfPoints, fromSourceTerminal);
                            }
                        }

                        return WireVerticallyTowardsEnd(start, end, bannedDirectionForEnd, pointsSoFar, uturnCount, maxNumberOfPoints, fromSourceTerminal);
                    }
                    break;

                case Direction.East:
                    // The end is to the right of the start
                    if (start.X >= end.X)
                    {
                        if (bannedDirectionForEnd == Direction.North)
                        {
                            if (start.Y > end.Y)
                            {
                                return WireHorizontiallyTowardsEnd(start, end, bannedDirectionForEnd, pointsSoFar, uturnCount, maxNumberOfPoints, fromSourceTerminal);
                            }
                            else
                            {
                                return WireVerticallyTowardsEnd(start, end, bannedDirectionForEnd, pointsSoFar, uturnCount, maxNumberOfPoints, fromSourceTerminal);
                            }
                        }

                        if (bannedDirectionForEnd == Direction.South)
                        {
                            if (start.Y > end.Y)
                            {
                                return WireVerticallyTowardsEnd(start, end, bannedDirectionForEnd, pointsSoFar, uturnCount, maxNumberOfPoints, fromSourceTerminal);
                            }
                            else
                            {
                                return WireHorizontiallyTowardsEnd(start, end, bannedDirectionForEnd, pointsSoFar, uturnCount, maxNumberOfPoints, fromSourceTerminal);
                            }
                        }

                        return WireHorizontiallyTowardsEnd(start, end, bannedDirectionForEnd, pointsSoFar, uturnCount, maxNumberOfPoints, fromSourceTerminal);
                    }
                    break;
            }

            // UTurn if nothing else works
            return UTurn(start, end, bannedDirectionForStart, bannedDirectionForEnd, pointsSoFar, uturnCount, maxNumberOfPoints, fromSourceTerminal);
        }

        private bool _uTurned = false;

        private IList<Point> UTurn(Point start, Point end, Direction bannedDirectionForStart, Direction bannedDirectionForEnd, IList<Point> pointsSoFar, int uturnCount, int maxNumberOfPoints, bool fromSourceTerminal)
        {
            _uTurned = true;
            if (uturnCount++ > 10)
            {
                while (pointsSoFar.Count > 1)
                {
                    pointsSoFar.RemoveAt(pointsSoFar.Count - 1);
                }

                return pointsSoFar;
            }

            Point newPoint;
            Direction newBannedDirection;
            if (bannedDirectionForStart == Direction.North || bannedDirectionForStart == Direction.South)
            {
                if (start.X < end.X)
                {
                    // Going east.
                    var sink = fromSourceTerminal ? RightUTurnLengthSource : RightUTurnLengthSink;
                    newPoint = new Point(start.X + sink, start.Y);
                    newBannedDirection = Direction.West;
                }
                else
                {
                    // Going west.
                    var sink = fromSourceTerminal ? LeftUTurnLengthSource : LeftUTurnLengthSink;
                    newPoint = new Point(start.X - sink, start.Y);
                    newBannedDirection = Direction.East;
                }
            }
            else
            {
                if (start.Y < end.Y)
                {
                    // Going south.
                    var sink = fromSourceTerminal ? DownUTurnLengthSource : DownUTurnLengthSink;
                    newPoint = new Point(start.X, start.Y + sink);
                    newBannedDirection = Direction.North;
                }
                else
                {
                    // Going north.
                    var sink = fromSourceTerminal ? UpUTurnLengthSource : UpUTurnLengthSink;
                    newPoint = new Point(start.X, start.Y - sink);
                    newBannedDirection = Direction.South;
                }
            }

            return WireTwoPoints(newPoint, end, newBannedDirection, pointsSoFar, bannedDirectionForEnd, uturnCount, maxNumberOfPoints, fromSourceTerminal);
        }

        private Direction OppositeDirection(Direction direction)
        {
            if (direction == Direction.North)
            {
                return Direction.South;
            }

            if (direction == Direction.South)
            {
                return Direction.North;
            }

            if (direction == Direction.East)
            {
                return Direction.West;
            }

            return Direction.East;
        }

        private IList<Point> WireHorizontiallyTowardsEnd(Point start, Point end, Direction bannedDirectionForEnd, IList<Point> pointsSoFar, int uturnCount, int maxNumberOfPoints, bool fromSourceTerminal)
        {
            var bannedStart = start.X < end.X ? Direction.West : Direction.East;
            var newPoint = new Point(end.X, start.Y);
            return WireTwoPoints(newPoint, end, bannedStart, bannedDirectionForEnd, pointsSoFar, uturnCount, fromSourceTerminal);
        }

        private IList<Point> WireVerticallyTowardsEnd(Point start, Point end, Direction bannedDirectionForEnd, IList<Point> pointsSoFar, int uturnCount, int maxNumberOfPoints, bool fromSourceTerminal)
        {
            var bannedStart = start.Y < end.Y ? Direction.North : Direction.South;
            var newPoint = new Point(start.X, end.Y);
            return WireTwoPoints(newPoint, end, bannedStart, bannedDirectionForEnd, pointsSoFar, uturnCount, fromSourceTerminal);
        }
    }
}
