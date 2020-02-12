using System.Collections.Generic;
using System.Windows;

namespace DiiagramrAPI.Editor.Diagrams
{
    public class SpacedPointsAlongLine
    {
        private readonly IList<Point> _linePoints;
        private readonly double _constantOffset;
        private readonly int _numberOfPoints;
        private readonly bool _addOriginalPointsToSpacedPoints;
        private IList<Point> _spacedPoints;
        private double _lengthOfLine;

        public SpacedPointsAlongLine(IList<Point> linePoints, double constantOffset, int numberOfPoints, bool addOriginalPointsToSpacedPoints = false)
        {
            _linePoints = linePoints;
            _constantOffset = constantOffset;
            _numberOfPoints = numberOfPoints;
            _addOriginalPointsToSpacedPoints = addOriginalPointsToSpacedPoints;
        }

        public IList<Point> SpacedPoints => _spacedPoints ?? (_spacedPoints = GetSpacedPointsAlongLine());

        public double LengthOfLine => _lengthOfLine > 0.01 ? _lengthOfLine : _lengthOfLine = GetLengthOfLine();

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
                    var targetLength = LengthOfLine * precentAlongLine;
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