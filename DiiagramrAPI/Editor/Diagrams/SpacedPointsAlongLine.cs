using System.Collections.Generic;
using System.Windows;

namespace DiiagramrAPI.Editor.Diagrams
{
    /// <summary>
    /// Class that is capable of generating specifically spaced points along a poly line.
    /// </summary>
    public class SpacedPointsAlongLine
    {
        private readonly IList<Point> _linePoints;
        private readonly double _constantOffset;
        private readonly int _numberOfPoints;
        private readonly bool _addOriginalPointsToSpacedPoints;
        private IList<Point> _spacedPoints;
        private double _lengthOfLine;

        /// <summary>
        /// Creates a new instance of <see cref="SpacedPointsAlongLine"/>.
        /// </summary>
        /// <param name="linePoints">A list of points describing the line to generate points along.</param>
        /// <param name="constantOffset">A constant distance to offset the generated points by from the ends of the line.</param>
        /// <param name="numberOfPoints">The number of points to generate.</param>
        /// <param name="addOriginalPointsToSpacedPoints">If true, <see cref="SpacedPoints"/> will contain <paramref name="linePoints"/>.</param>
        public SpacedPointsAlongLine(IList<Point> linePoints, double constantOffset, int numberOfPoints, bool addOriginalPointsToSpacedPoints = false)
        {
            _linePoints = linePoints;
            _constantOffset = constantOffset;
            _numberOfPoints = numberOfPoints;
            _addOriginalPointsToSpacedPoints = addOriginalPointsToSpacedPoints;
            _lengthOfLine = GetLengthOfLine();
        }

        /// <summary>
        /// The calculated points.
        /// </summary>
        public IList<Point> SpacedPoints => _spacedPoints ?? (_spacedPoints = GetSpacedPointsAlongLine());

        private static Point GetInterpolatedPoint(Point point1, Point point2, double desiredPrecentOfSegment)
        {
            var diff = Point.Subtract(point2, point1);
            var interpolatedX = point1.X + diff.X * desiredPrecentOfSegment;
            var interpolatedY = point1.Y + diff.Y * desiredPrecentOfSegment;
            var interpolatedPoint = new Point(interpolatedX, interpolatedY);
            return interpolatedPoint;
        }

        private Point OffsetPoint(Point p) => new Point(p.X - _constantOffset, p.Y - _constantOffset);

        private IList<Point> GetSpacedPointsAlongLine()
        {
            int temp = 0;
            var spacedPoints = new List<Point>();
            if (_linePoints == null || _linePoints.Count == 0)
            {
                return spacedPoints;
            }

            var incrementPrecent = 1.0 / (_numberOfPoints - 1);
            for (double precentAlongLine = 0.0; precentAlongLine < 1.0 + (incrementPrecent / 2.0); precentAlongLine += incrementPrecent)
            {
                var distanceSoFar = 0.0;
                for (int pointIndex = 0; pointIndex < _linePoints.Count - 1; pointIndex++)
                {
                    var segmentStartPoint = _linePoints[pointIndex];
                    var segmentEndPoint = _linePoints[pointIndex + 1];
                    var nextSegmentAvailableLength = Point.Subtract(segmentStartPoint, segmentEndPoint).Length;
                    var targetLength = _lengthOfLine * precentAlongLine;
                    var nextSegmentDesiredLength = targetLength - distanceSoFar;
                    if (nextSegmentAvailableLength > nextSegmentDesiredLength)
                    {
                        var desiredPrecentOfNextLength = nextSegmentDesiredLength / nextSegmentAvailableLength;
                        var interpolatedPoint = GetInterpolatedPoint(segmentStartPoint, segmentEndPoint, desiredPrecentOfNextLength);
                        spacedPoints.Add(OffsetPoint(interpolatedPoint));
                        break;
                    }
                    if (pointIndex >= temp && _addOriginalPointsToSpacedPoints)
                    {
                        spacedPoints.Add(OffsetPoint(_linePoints[pointIndex + 1]));
                        temp++;
                    }
                    distanceSoFar += nextSegmentAvailableLength;
                }
            }
            return spacedPoints;
        }

        private double GetLengthOfLine()
        {
            var length = 0.0;
            for (int i = 0; i < _linePoints.Count - 1; i++)
            {
                length += Point.Subtract(_linePoints[i], _linePoints[i + 1]).Length;
            }
            return length;
        }
    }
}