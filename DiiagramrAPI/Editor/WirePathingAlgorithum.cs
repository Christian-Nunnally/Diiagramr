using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;
using System;
using System.Collections.Generic;
using System.Windows;

namespace DiiagramrAPI.Editor
{
    public class WirePathingAlgorithum
    {
        private const int MinimimDistanceToCalculateWire = 50;
        private bool _uTurned = false;

        public bool EnableUTurnLimitsForSourceTerminal { get; set; } = true;

        public bool EnableUTurnLimitsForSinkTerminal { get; set; } = true;

        public float WireDistanceOutOfSourceTerminal { get; set; } = 25.0f;

        public float WireDistanceOutOfSinkTerminal { get; set; } = 25.0f;

        private double DownUTurnLengthSink { get; set; }

        private double DownUTurnLengthSource { get; set; }

        private double LeftUTurnLengthSink { get; set; }

        private double LeftUTurnLengthSource { get; set; }

        private double RightUTurnLengthSink { get; set; }

        private double RightUTurnLengthSource { get; set; }

        private double UpUTurnLengthSink { get; set; }

        private double UpUTurnLengthSource { get; set; }

        public Point[] GetWirePoints(Wire wire)
        {
            InitializeUTurnLengths(wire);
            var start = new Point(wire.X2, wire.Y2);
            var end = new Point(wire.X1, wire.Y1);
            var sinkTerminalSide = wire.WireModel.SinkTerminal.DefaultSide;
            var sourceTerminalSide = wire.WireModel.SourceTerminal.DefaultSide;
            var startTerminalBannedDirection = sinkTerminalSide.Opposite();
            var endTerminalBannedDirection = sourceTerminalSide.Opposite();
            var stubStart = TranslatePointInDirection(start, sinkTerminalSide, WireDistanceOutOfSinkTerminal);
            var stubEnd = TranslatePointInDirection(end, sourceTerminalSide, WireDistanceOutOfSourceTerminal);

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

                case Direction.None:
                default:
                    return new Point(p.X, p.Y);
            }
        }

        private (double DownWireMinimumLength, double LeftWireMinimumLength, double RightWireMinimumLength, double UpWireMinimumLength) CalculateUTurnLimitsForTerminal(TerminalModel terminal)
        {
            const double marginFromEdgeOfNode = Diagram.NodeBorderWidth + 10.0;
            const double halfTerminalHeight = Terminal.TerminalHeight / 2.0;
            var nodeHeight = terminal.ParentNode?.Height ?? 0;
            var nodeWidth = terminal.ParentNode?.Width ?? 0;
            var offsetX = terminal.OffsetX;
            var offsetY = terminal.OffsetY;
            var terminalDirection = terminal.DefaultSide;
            double terminalUpWireMinimumLength = 0;
            double terminalDownWireMinimumLength = 0;
            double terminalLeftWireMinimumLength = 0;
            double terminalRightWireMinimumLength = 0;
            if (terminalDirection == Direction.North)
            {
                terminalUpWireMinimumLength = marginFromEdgeOfNode;
                terminalDownWireMinimumLength = marginFromEdgeOfNode + marginFromEdgeOfNode + nodeHeight;
                terminalLeftWireMinimumLength = marginFromEdgeOfNode + offsetX - halfTerminalHeight;
                terminalRightWireMinimumLength = marginFromEdgeOfNode + (nodeWidth - offsetX) + halfTerminalHeight;
            }
            else if (terminalDirection == Direction.South)
            {
                terminalUpWireMinimumLength = marginFromEdgeOfNode + marginFromEdgeOfNode + nodeHeight;
                terminalDownWireMinimumLength = marginFromEdgeOfNode;
                terminalLeftWireMinimumLength = marginFromEdgeOfNode + offsetX - halfTerminalHeight;
                terminalRightWireMinimumLength = marginFromEdgeOfNode + (nodeWidth - offsetX) + halfTerminalHeight;
            }
            else if (terminalDirection == Direction.East)
            {
                terminalUpWireMinimumLength = marginFromEdgeOfNode + offsetY - halfTerminalHeight;
                terminalDownWireMinimumLength = marginFromEdgeOfNode + (nodeHeight - offsetY) + halfTerminalHeight;
                terminalLeftWireMinimumLength = marginFromEdgeOfNode + marginFromEdgeOfNode + nodeWidth;
                terminalRightWireMinimumLength = marginFromEdgeOfNode;
            }
            else if (terminalDirection == Direction.West)
            {
                terminalUpWireMinimumLength = marginFromEdgeOfNode + offsetY - halfTerminalHeight;
                terminalDownWireMinimumLength = marginFromEdgeOfNode + (nodeHeight - offsetY) + halfTerminalHeight;
                terminalLeftWireMinimumLength = marginFromEdgeOfNode;
                terminalRightWireMinimumLength = marginFromEdgeOfNode + marginFromEdgeOfNode + nodeWidth;
            }
            return (terminalDownWireMinimumLength, terminalLeftWireMinimumLength, terminalRightWireMinimumLength, terminalUpWireMinimumLength);
        }

        private void InitializeUTurnLengths(Wire wire)
        {
            if (wire.WireModel != null)
            {
                if (wire.WireModel.SourceTerminal != null && EnableUTurnLimitsForSourceTerminal)
                {
                    var UTurnLimits = CalculateUTurnLimitsForTerminal(wire.WireModel.SourceTerminal);
                    DownUTurnLengthSource = UTurnLimits.DownWireMinimumLength;
                    LeftUTurnLengthSource = UTurnLimits.LeftWireMinimumLength;
                    RightUTurnLengthSource = UTurnLimits.RightWireMinimumLength;
                    UpUTurnLengthSource = UTurnLimits.UpWireMinimumLength;
                }
                else
                {
                    DownUTurnLengthSource = 0;
                    LeftUTurnLengthSource = 0;
                    RightUTurnLengthSource = 0;
                    UpUTurnLengthSource = 0;
                }
                if (wire.WireModel.SinkTerminal != null && EnableUTurnLimitsForSinkTerminal)
                {
                    var UTurnLimits = CalculateUTurnLimitsForTerminal(wire.WireModel.SinkTerminal);
                    DownUTurnLengthSink = UTurnLimits.DownWireMinimumLength;
                    LeftUTurnLengthSink = UTurnLimits.LeftWireMinimumLength;
                    RightUTurnLengthSink = UTurnLimits.RightWireMinimumLength;
                    UpUTurnLengthSink = UTurnLimits.UpWireMinimumLength;
                }
                else
                {
                    DownUTurnLengthSink = 0;
                    LeftUTurnLengthSink = 0;
                    RightUTurnLengthSink = 0;
                    UpUTurnLengthSink = 0;
                }
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

                case Direction.East:
                case Direction.West:
                case Direction.None:
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

                case Direction.North:
                case Direction.South:
                case Direction.None:
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