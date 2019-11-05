using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;
using System;
using System.Collections.Generic;
using System.Windows;

namespace DiiagramrAPI.Editor
{
    internal class WirePathingAlgorithum
    {
        private const int MinimimDistanceToCalculateWire = 50;
        private const double WireDistanceOutOfTerminal = 25.0;
        private bool _uTurned = false;
        private WireModel _wireModel;

        public Terminal FallbackSinkTerminal { get; internal set; }

        public Terminal FallbackSourceTerminal { get; internal set; }

        private TerminalModel SinkTerminal => _wireModel?.SinkTerminal;

        private TerminalModel SourceTerminal => _wireModel?.SourceTerminal;

        private double DownUTurnLengthSink => _wireModel != null ? SinkTerminal.TerminalDownWireMinimumLength : 0;

        private double DownUTurnLengthSource => _wireModel != null ? SourceTerminal.TerminalDownWireMinimumLength : (FallbackSinkTerminal?.TerminalDownWireMinimumLength ?? (FallbackSourceTerminal?.TerminalDownWireMinimumLength ?? 0));

        private double LeftUTurnLengthSink => _wireModel != null ? SinkTerminal.TerminalLeftWireMinimumLength : 0;

        private double LeftUTurnLengthSource => _wireModel != null ? SourceTerminal.TerminalLeftWireMinimumLength : (FallbackSinkTerminal?.TerminalLeftWireMinimumLength ?? (FallbackSourceTerminal?.TerminalLeftWireMinimumLength ?? 0));

        private double RightUTurnLengthSink => _wireModel != null ? SinkTerminal.TerminalRightWireMinimumLength : 0;

        private double RightUTurnLengthSource => _wireModel != null ? SourceTerminal.TerminalRightWireMinimumLength : (FallbackSinkTerminal?.TerminalRightWireMinimumLength ?? (FallbackSourceTerminal?.TerminalRightWireMinimumLength ?? 0));

        private double UpUTurnLengthSink => _wireModel != null ? SinkTerminal.TerminalUpWireMinimumLength : 0;

        private double UpUTurnLengthSource => _wireModel != null ? SourceTerminal.TerminalUpWireMinimumLength : (FallbackSinkTerminal?.TerminalUpWireMinimumLength ?? (FallbackSourceTerminal?.TerminalUpWireMinimumLength ?? 0));

        public Point[] GetWirePoints(WireModel model, double x1, double y1, double x2, double y2, Direction startTerminalDefaultDirection, Direction endTerminalDefaultDirection)
        {
            _wireModel = model;
            var start = new Point(x1, y1);
            var end = new Point(x2, y2);
            var sinkTerminalSide = SinkTerminal?.DefaultSide ?? startTerminalDefaultDirection.Opposite();
            var sourceTerminalSide = SourceTerminal?.DefaultSide ?? endTerminalDefaultDirection.Opposite();
            var startTerminalBannedDirection = sinkTerminalSide.Opposite();
            var endTerminalBannedDirection = sourceTerminalSide.Opposite();
            var stubStart = TranslatePointInDirection(start, sinkTerminalSide, WireDistanceOutOfTerminal);
            var stubEnd = TranslatePointInDirection(end, sourceTerminalSide, WireDistanceOutOfTerminal);

            if (ArePointsWithinDistance(stubStart, stubEnd, MinimimDistanceToCalculateWire))
            {
                return new Point[] { start, stubStart, stubEnd, end };
            }

            _uTurned = false;
            var backwardPoints = new List<Point> { end };
            WireTwoPoints(stubEnd, stubStart, endTerminalBannedDirection, startTerminalBannedDirection, backwardPoints, true);
            if (_uTurned)
            {
                endTerminalBannedDirection = GetBannedDirectionFromPoints(backwardPoints[1], backwardPoints[2]);
                stubEnd = backwardPoints[2];
            }

            var points = new List<Point> { start };
            WireTwoPoints(stubStart, stubEnd, startTerminalBannedDirection, endTerminalBannedDirection, points, false);

            if (_uTurned)
            {
                points.Add(backwardPoints[1]);
            }

            points.Add(end);
            return points.ToArray();
        }

        private static Direction GetBannedDirectionFromPoints(Point start, Point end)
        {
            var deltaX = start.X - end.X;
            var deltaY = start.Y - end.Y;
            var bannedVerticalDirection = deltaX > 0 ? Direction.East : Direction.West;
            var bannedHorizontialDirection = deltaY > 0 ? Direction.South : Direction.North;
            return Math.Abs(deltaX) > Math.Abs(deltaY) ? bannedVerticalDirection : bannedHorizontialDirection;
        }

        private static Point TranslatePointInDirection(Point p, Direction direction, double amount)
        {
            switch (direction)
            {
                case Direction.North:
                    return new Point(p.X, p.Y - amount);

                case Direction.South:
                    return new Point(p.X, p.Y + amount);

                case Direction.East:
                    return new Point(p.X + amount, p.Y);

                case Direction.West:
                    return new Point(p.X - amount, p.Y);

                default:
                    return new Point(p.X, p.Y);
            }
        }

        private bool ArePointsWithinDistance(Point start, Point end, float distance)
        {
            return distance > Math.Abs(start.X - end.X) + Math.Abs(start.Y - end.Y);
        }

        private List<Point> UTurn(Point start, Point end, Direction bannedStartDirection, Direction bannedEndDirection, List<Point> pointsSoFar, bool wireBackwards)
        {
            _uTurned = true;
            Point newPoint;
            Direction newBannedDirection;
            if (bannedStartDirection == Direction.North || bannedStartDirection == Direction.South)
            {
                var goingEast = start.X < end.X;
                if (goingEast)
                {
                    var length = wireBackwards ? RightUTurnLengthSource : RightUTurnLengthSink;
                    newPoint = new Point(start.X + length, start.Y);
                    newBannedDirection = Direction.West;
                }
                else
                {
                    var length = wireBackwards ? LeftUTurnLengthSource : LeftUTurnLengthSink;
                    newPoint = new Point(start.X - length, start.Y);
                    newBannedDirection = Direction.East;
                }
            }
            else
            {
                var goingSouth = start.Y < end.Y;
                if (goingSouth)
                {
                    var length = wireBackwards ? DownUTurnLengthSource : DownUTurnLengthSink;
                    newPoint = new Point(start.X, start.Y + length);
                    newBannedDirection = Direction.North;
                }
                else
                {
                    var length = wireBackwards ? UpUTurnLengthSource : UpUTurnLengthSink;
                    newPoint = new Point(start.X, start.Y - length);
                    newBannedDirection = Direction.South;
                }
            }

            return WireTwoPoints(newPoint, end, newBannedDirection, bannedEndDirection, pointsSoFar, wireBackwards);
        }

        private List<Point> WireHorizontiallyTowardsEnd(Point start, Point end, Direction bannedEndDirection, List<Point> pointsSoFar, bool wireBackwards)
        {
            var bannedStart = start.X < end.X ? Direction.West : Direction.East;
            var newPoint = new Point(end.X, start.Y);
            return WireTwoPoints(newPoint, end, bannedStart, bannedEndDirection, pointsSoFar, wireBackwards);
        }

        private List<Point> WireToEndRespectingEastWestStartPoint(Point start, Point end, List<Point> pointsSoFar, Direction bannedEndDirection, bool wireBackwards, bool isEndAboveStart)
        {
            switch (bannedEndDirection)
            {
                case Direction.North:
                    return WireTowardsEnd(start, end, pointsSoFar, bannedEndDirection, wireBackwards, !isEndAboveStart);

                case Direction.South:
                    return WireTowardsEnd(start, end, pointsSoFar, bannedEndDirection, wireBackwards, isEndAboveStart);

                default:
                    return WireHorizontiallyTowardsEnd(start, end, bannedEndDirection, pointsSoFar, wireBackwards);
            }
        }

        private List<Point> WireToEndRespectingNorthSouthStartPoint(Point start, Point end, List<Point> pointsSoFar, Direction bannedEndDirection, bool wireBackwards, bool isEndLeftOfStart)
        {
            switch (bannedEndDirection)
            {
                case Direction.East:
                    return WireTowardsEnd(start, end, pointsSoFar, bannedEndDirection, wireBackwards, !isEndLeftOfStart);

                case Direction.West:
                    return WireTowardsEnd(start, end, pointsSoFar, bannedEndDirection, wireBackwards, isEndLeftOfStart);

                default:
                    return WireVerticallyTowardsEnd(start, end, bannedEndDirection, pointsSoFar, wireBackwards);
            }
        }

        private List<Point> WireTowardsEnd(Point start, Point end, List<Point> pointsSoFar, Direction bannedEndDirection, bool wireBackwards, bool wireVertically)
        {
            return wireVertically
                ? WireVerticallyTowardsEnd(start, end, bannedEndDirection, pointsSoFar, wireBackwards)
                : WireHorizontiallyTowardsEnd(start, end, bannedEndDirection, pointsSoFar, wireBackwards);
        }

        private List<Point> WireTwoPoints(Point start, Point end, Direction bannedStartDirection, Direction bannedEndDirection, List<Point> pointsSoFar, bool wireBackwards)
        {
            pointsSoFar.Add(start);

            if (Math.Abs(start.Y - end.Y) < 0.5 || Math.Abs(start.X - end.X) < 0.5)
            {
                pointsSoFar.Add(end);
                return pointsSoFar;
            }

            var isEndAboveStart = start.Y > end.Y;
            var isEndLeftOfStart = start.X > end.X;
            if ((bannedStartDirection == Direction.South && isEndAboveStart)
             || (bannedStartDirection == Direction.North && !isEndAboveStart))
            {
                return WireToEndRespectingNorthSouthStartPoint(start, end, pointsSoFar, bannedEndDirection, wireBackwards, isEndLeftOfStart);
            }

            if ((bannedStartDirection == Direction.East && isEndLeftOfStart)
             || (bannedStartDirection == Direction.West && !isEndLeftOfStart))
            {
                return WireToEndRespectingEastWestStartPoint(start, end, pointsSoFar, bannedEndDirection, wireBackwards, isEndAboveStart);
            }

            // UTurn if nothing else works.
            return UTurn(start, end, bannedStartDirection, bannedEndDirection, pointsSoFar, wireBackwards);
        }

        private List<Point> WireVerticallyTowardsEnd(Point start, Point end, Direction bannedEndDirection, List<Point> pointsSoFar, bool wireBackwards)
        {
            var bannedStart = start.Y < end.Y ? Direction.North : Direction.South;
            var newPoint = new Point(start.X, end.Y);
            return WireTwoPoints(newPoint, end, bannedStart, bannedEndDirection, pointsSoFar, wireBackwards);
        }
    }
}