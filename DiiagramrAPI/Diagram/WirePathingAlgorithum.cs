using DiiagramrAPI.Service;
using DiiagramrModel;
using System;
using System.Collections.Generic;
using System.Windows;

namespace DiiagramrAPI.Diagram
{
    internal class WirePathingAlgorithum
    {
        private const double WireDistanceOutOfTerminal = 25.0;
        private bool _uTurned = false;
        private const int MinimimDistanceToCalculateWire = 50;
        private WireModel _wireModel;

        private double DownUTurnLengthSink => _wireModel != null ? _wireModel.SinkTerminal.TerminalDownWireMinimumLength : 0;
        private double DownUTurnLengthSource => _wireModel != null ? _wireModel.SourceTerminal.TerminalDownWireMinimumLength : (FallbackSinkTerminal?.TerminalDownWireMinimumLength ?? (FallbackSourceTerminal?.TerminalDownWireMinimumLength ?? 0));
        private double LeftUTurnLengthSink => _wireModel != null ? _wireModel.SinkTerminal.TerminalLeftWireMinimumLength : 0;
        private double LeftUTurnLengthSource => _wireModel != null ? _wireModel.SourceTerminal.TerminalLeftWireMinimumLength : (FallbackSinkTerminal?.TerminalLeftWireMinimumLength ?? (FallbackSourceTerminal?.TerminalLeftWireMinimumLength ?? 0));
        private double RightUTurnLengthSink => _wireModel != null ? _wireModel.SinkTerminal.TerminalRightWireMinimumLength : 0;
        private double RightUTurnLengthSource => _wireModel != null ? _wireModel.SourceTerminal.TerminalRightWireMinimumLength : (FallbackSinkTerminal?.TerminalRightWireMinimumLength ?? (FallbackSourceTerminal?.TerminalRightWireMinimumLength ?? 0));
        private double UpUTurnLengthSink => _wireModel != null ? _wireModel.SinkTerminal.TerminalUpWireMinimumLength : 0;
        private double UpUTurnLengthSource => _wireModel != null ? _wireModel.SourceTerminal.TerminalUpWireMinimumLength : (FallbackSinkTerminal?.TerminalUpWireMinimumLength ?? (FallbackSourceTerminal?.TerminalUpWireMinimumLength ?? 0));

        public Point[] GetWirePoints(WireModel model, double x1, double y1, double x2, double y2, Direction bannedDirectionForStart, Direction bannedDirectionForEnd)
        {
            _wireModel = model;
            var start = new Point(x1, y1);
            var end = new Point(x2, y2);

            var stubStart = TranslatePointInDirection(start, bannedDirectionForStart.Opposite(), WireDistanceOutOfTerminal);
            var stubEnd = TranslatePointInDirection(end, bannedDirectionForEnd.Opposite(), WireDistanceOutOfTerminal);
            if (_wireModel != null)
            {
                bannedDirectionForStart = _wireModel.SinkTerminal.DefaultSide.Opposite();
                bannedDirectionForEnd = _wireModel.SourceTerminal.DefaultSide.Opposite();
                stubStart = TranslatePointInDirection(start, _wireModel.SinkTerminal.DefaultSide, WireDistanceOutOfTerminal);
                stubEnd = TranslatePointInDirection(end, _wireModel.SourceTerminal.DefaultSide, WireDistanceOutOfTerminal);
            }
            if (StubsAreTooCloseTogether(stubStart, stubEnd))
            {
                return new Point[] { start, stubStart, stubEnd, end };
            }

            _uTurned = false;
            var backwardPoints = new List<Point> { end };
            WireTwoPoints(stubEnd, stubStart, bannedDirectionForEnd, bannedDirectionForStart, backwardPoints, 2, true);
            if (_uTurned)
            {
                bannedDirectionForEnd = GetBannedDirectionFromPoints(backwardPoints[1], backwardPoints[2]);
                stubEnd = backwardPoints[2];
            }

            var points = new List<Point> { start };
            WireTwoPoints(stubStart, stubEnd, bannedDirectionForStart, bannedDirectionForEnd, points);

            if (_uTurned)
            {
                points.Add(backwardPoints[1]);
            }
            points.Add(end);
            return points.ToArray();
        }

        public Terminal FallbackSourceTerminal { get; internal set; }
        public Terminal FallbackSinkTerminal { get; internal set; }

        private bool StubsAreTooCloseTogether(Point stubStart, Point stubEnd)
        {
            return MinimimDistanceToCalculateWire > Math.Abs(stubStart.X - stubEnd.X) + Math.Abs(stubStart.Y - stubEnd.Y);
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

        private List<Point> UTurn(Point start, Point end, Direction bannedDirectionForStart, Direction bannedDirectionForEnd, List<Point> pointsSoFar, int uturnCount, int maxNumberOfPoints, bool fromSourceTerminal)
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

        private List<Point> WireHorizontiallyTowardsEnd(Point start, Point end, Direction bannedDirectionForEnd, List<Point> pointsSoFar, int uturnCount, int maxNumberOfPoints, bool fromSourceTerminal)
        {
            var bannedStart = start.X < end.X ? Direction.West : Direction.East;
            var newPoint = new Point(end.X, start.Y);
            return WireTwoPoints(newPoint, end, bannedStart, bannedDirectionForEnd, pointsSoFar, uturnCount, fromSourceTerminal);
        }

        private List<Point> WireTwoPoints(Point start, Point end, Direction bannedDirectionForStart, Direction bannedDirectionForEnd, List<Point> pointsSoFar)
        {
            return WireTwoPoints(start, end, bannedDirectionForStart, pointsSoFar, bannedDirectionForEnd, 0, -1, false);
        }

        private List<Point> WireTwoPoints(Point start, Point end, Direction bannedDirectionForStart, Direction bannedDirectionForEnd, List<Point> pointsSoFar, int maxNumberOfPoints, bool fromSourceTerminal)
        {
            return WireTwoPoints(start, end, bannedDirectionForStart, pointsSoFar, bannedDirectionForEnd, 0, maxNumberOfPoints, fromSourceTerminal);
        }

        private List<Point> WireTwoPoints(Point start, Point end, Direction bannedDirectionForStart, List<Point> pointsSoFar, Direction bannedDirectionForEnd, int uturnCount, int maxNumberOfPoints, bool fromSourceTerminal)
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

        private List<Point> WireVerticallyTowardsEnd(Point start, Point end, Direction bannedDirectionForEnd, List<Point> pointsSoFar, int uturnCount, int maxNumberOfPoints, bool fromSourceTerminal)
        {
            var bannedStart = start.Y < end.Y ? Direction.North : Direction.South;
            var newPoint = new Point(start.X, end.Y);
            return WireTwoPoints(newPoint, end, bannedStart, bannedDirectionForEnd, pointsSoFar, uturnCount, fromSourceTerminal);
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

            if (direction == Direction.East)
            {
                return new Point(p.X + amount, p.Y);
            }

            return direction == Direction.West ? new Point(p.X - amount, p.Y) : new Point(p.X, p.Y);
        }
    }
}
