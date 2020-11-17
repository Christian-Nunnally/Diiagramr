using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;
using System;
using System.Collections.Generic;
using System.Windows;

namespace DiiagramrAPI.Editor
{
    /// <summary>
    /// Quickly paths a wire from a start point to an end point.
    /// </summary>
    public class WirePathingAlgorithum
    {
        private readonly int _minimimDistanceToCalculateWire = 50;

        private bool _uTurned = false;

        private double _downUTurnLengthSink;

        private double _downUTurnLengthSource;

        private double _leftUTurnLengthSink;

        private double _leftUTurnLengthSource;

        private double _rightUTurnLengthSink;

        private double _rightUTurnLengthSource;

        private double _upUTurnLengthSink;

        private double _upUTurnLengthSource;

        /// <summary>
        /// Gets or sets whether to limit the number of times the wire can take a u turn out of the source terminal.
        /// </summary>
        public bool EnableUTurnLimitsForSourceTerminal { get; set; } = true;

        /// <summary>
        /// Gets or sets whether to limit the number of times the wire can take a u turn out of the sink terminal.
        /// </summary>
        public bool EnableUTurnLimitsForSinkTerminal { get; set; } = true;

        /// <summary>
        /// Gets or sets the distance the wire should travel out of the source terminal before starting to path.
        /// </summary>
        public float WireDistanceOutOfSourceTerminal { get; set; } = 25.0f;

        /// <summary>
        /// Gets or sets the distance the wire should travel out of the sink terminal before starting to path.
        /// </summary>
        public float WireDistanceOutOfSinkTerminal { get; set; } = 25.0f;

        /// <summary>
        /// Generate the wire path points for a given <see cref="Wire"/>.
        /// </summary>
        /// <param name="wire">The wire to generate the path of.</param>
        /// <returns>The path of the wire as an array of points.</returns>
        public Point[] GetWirePoints(Wire wire)
        {
            InitializeUTurnLengths(wire);
            var sink = new Point(wire.X2, wire.Y2);
            var source = new Point(wire.X1, wire.Y1);
            var sinkTerminalSide = wire.WireModel.SinkTerminal.DefaultSide;
            var sourceTerminalSide = wire.WireModel.SourceTerminal.DefaultSide;
            var sinkTerminalBannedDirection = sinkTerminalSide.Opposite();
            var sourceTerminalBannedDirection = sourceTerminalSide.Opposite();
            var sinkStub = TranslatePointInDirection(sink, sinkTerminalSide, WireDistanceOutOfSinkTerminal);
            var sourceStub = TranslatePointInDirection(source, sourceTerminalSide, WireDistanceOutOfSourceTerminal);

            if (IsStubInsideNode(sourceStub, sinkStub))
            {
                return new Point[] { sink, sinkStub, sourceStub, source };
            }

            if (ArePointsWithinDistance(sinkStub, sourceStub, _minimimDistanceToCalculateWire))
            {
                return new Point[] { sink, sinkStub, sourceStub, source };
            }

            _uTurned = false;
            var backwardPoints = new List<Point> { source };
            WireTwoPoints(sourceStub, sinkStub, sourceTerminalBannedDirection, sinkTerminalBannedDirection, backwardPoints, true);
            if (_uTurned)
            {
                sourceTerminalBannedDirection = GetBannedDirectionFromPoints(backwardPoints[1], backwardPoints[2]);
                sourceStub = backwardPoints[2];
            }

            var points = new List<Point> { sink };
            WireTwoPoints(sinkStub, sourceStub, sinkTerminalBannedDirection, sourceTerminalBannedDirection, points, false);

            if (_uTurned)
            {
                points.Add(backwardPoints[1]);
            }

            points.Add(source);
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

        private static Point TranslatePointInDirection(Point p, Direction direction, double amount) => direction switch
        {
            Direction.North => new Point(p.X, p.Y - amount),
            Direction.South => new Point(p.X, p.Y + amount),
            Direction.East => new Point(p.X + amount, p.Y),
            Direction.West => new Point(p.X - amount, p.Y),
            _ => new Point(p.X, p.Y),
        };

        private bool IsStubInsideNode(Point sourceStub, Point sinkStub)
        {
            return IsSourceInsideSinkNode(sourceStub, sinkStub)
                || IsSinkInsideSourceNode(sourceStub, sinkStub);
        }

        private bool IsSourceInsideSinkNode(Point sourceStub, Point sinkStub)
        {
            var nodePosition = new Point(sinkStub.X - _leftUTurnLengthSink, sinkStub.Y - _upUTurnLengthSink);
            var nodeSize = new Size(_leftUTurnLengthSink + _rightUTurnLengthSink, _upUTurnLengthSink + _downUTurnLengthSink);
            Rect nodeRectangle = new Rect(nodePosition, nodeSize);
            return EnableUTurnLimitsForSinkTerminal && nodeRectangle.Contains(sourceStub);
        }

        private bool IsSinkInsideSourceNode(Point sourceStub, Point sinkStub)
        {
            var nodePosition = new Point(sourceStub.X - _leftUTurnLengthSource, sourceStub.Y - _upUTurnLengthSource);
            var nodeSize = new Size(_leftUTurnLengthSource + _rightUTurnLengthSource, _upUTurnLengthSource + _downUTurnLengthSource);
            Rect nodeRectangle = new Rect(nodePosition, nodeSize);
            return EnableUTurnLimitsForSourceTerminal && nodeRectangle.Contains(sinkStub);
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
                    _downUTurnLengthSource = UTurnLimits.DownWireMinimumLength;
                    _leftUTurnLengthSource = UTurnLimits.LeftWireMinimumLength;
                    _rightUTurnLengthSource = UTurnLimits.RightWireMinimumLength;
                    _upUTurnLengthSource = UTurnLimits.UpWireMinimumLength;
                }
                else
                {
                    _downUTurnLengthSource = 0;
                    _leftUTurnLengthSource = 0;
                    _rightUTurnLengthSource = 0;
                    _upUTurnLengthSource = 0;
                }
                if (wire.WireModel.SinkTerminal != null && EnableUTurnLimitsForSinkTerminal)
                {
                    var UTurnLimits = CalculateUTurnLimitsForTerminal(wire.WireModel.SinkTerminal);
                    _downUTurnLengthSink = UTurnLimits.DownWireMinimumLength;
                    _leftUTurnLengthSink = UTurnLimits.LeftWireMinimumLength;
                    _rightUTurnLengthSink = UTurnLimits.RightWireMinimumLength;
                    _upUTurnLengthSink = UTurnLimits.UpWireMinimumLength;
                }
                else
                {
                    _downUTurnLengthSink = 0;
                    _leftUTurnLengthSink = 0;
                    _rightUTurnLengthSink = 0;
                    _upUTurnLengthSink = 0;
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
                    var length = wireBackwards ? _rightUTurnLengthSource : _rightUTurnLengthSink;
                    newPoint = new Point(start.X + length, start.Y);
                    newBannedDirection = Direction.West;
                }
                else
                {
                    var length = wireBackwards ? _leftUTurnLengthSource : _leftUTurnLengthSink;
                    newPoint = new Point(start.X - length, start.Y);
                    newBannedDirection = Direction.East;
                }
            }
            else
            {
                var goingSouth = start.Y < end.Y;
                if (goingSouth)
                {
                    var length = wireBackwards ? _downUTurnLengthSource : _downUTurnLengthSink;
                    newPoint = new Point(start.X, start.Y + length);
                    newBannedDirection = Direction.North;
                }
                else
                {
                    var length = wireBackwards ? _upUTurnLengthSource : _upUTurnLengthSink;
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

        private List<Point> WireToEndRespectingEastWestStartPoint(Point start, Point end, List<Point> pointsSoFar, Direction bannedEndDirection, bool wireBackwards, bool isEndAboveStart) => bannedEndDirection switch
        {
            Direction.North => WireTowardsEnd(start, end, pointsSoFar, bannedEndDirection, wireBackwards, !isEndAboveStart),
            Direction.South => WireTowardsEnd(start, end, pointsSoFar, bannedEndDirection, wireBackwards, isEndAboveStart),
            _ => WireHorizontiallyTowardsEnd(start, end, bannedEndDirection, pointsSoFar, wireBackwards),
        };

        private List<Point> WireToEndRespectingNorthSouthStartPoint(Point start, Point end, List<Point> pointsSoFar, Direction bannedEndDirection, bool wireBackwards, bool isEndLeftOfStart) => bannedEndDirection switch
        {
            Direction.East => WireTowardsEnd(start, end, pointsSoFar, bannedEndDirection, wireBackwards, !isEndLeftOfStart),
            Direction.West => WireTowardsEnd(start, end, pointsSoFar, bannedEndDirection, wireBackwards, isEndLeftOfStart),
            _ => WireVerticallyTowardsEnd(start, end, bannedEndDirection, pointsSoFar, wireBackwards),
        };

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