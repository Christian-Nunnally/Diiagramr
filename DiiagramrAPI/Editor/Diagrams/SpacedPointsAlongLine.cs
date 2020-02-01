using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace DiiagramrAPI.Editor.Diagrams
{
    public class SpacedPointsAlongLine
    {
        private readonly IList<Point> _linePoints;
        private readonly Point _constantOffset;

        public SpacedPointsAlongLine(IList<Point> linePoints, Point constantOffset)
        {
            _linePoints = linePoints;
            _constantOffset = constantOffset;
        }

        public IList<Point> SpacedPoints { get; } = new List<Point>();

        private static Point GetInterpolatedPoint(Point point1, Point point2, double desiredPrecentOfSegment)
        {
            var diff = Point.Subtract(point2, point1);
            var interpolatedX = point1.X + diff.X * desiredPrecentOfSegment;
            var interpolatedY = point1.Y + diff.Y * desiredPrecentOfSegment;
            var interpolatedPoint = new Point(interpolatedX, interpolatedY);
            return interpolatedPoint;
        }

        private void AddSpacedPoint(Point p)
        {
            var offsetPoint = new Point(p.X - _constantOffset.X, p.Y - _constantOffset.Y);
            SpacedPoints.Add(offsetPoint);
        }

        private void GenerateSpacedPointsAlongLine()
        {
            SpacedPoints.Clear();
            if (SpacedPoints == null || _linePoints.Count == 0)
            {
                return;
            }

            AddSpacedPoint(_linePoints.First());
            var totalLength = GetLengthOfWire(originalPoints);

            for (int frameNumber = 0; frameNumber < WireDataAnimationFrames; frameNumber++)
            {
                var lengthSoFar = 0.0;

                for (int j = 0; j < _linePoints.Count - 1; j++)
                {
                    var nextLength = Point.Subtract(_linePoints[j], _linePoints[j + 1]).Length;
                    var precentAlongAnimation = (double)frameNumber / WireDataAnimationFrames * (Math.PI / 2.0);
                    var targetLength = totalLength * Math.Sin(precentAlongAnimation);
                    if (lengthSoFar + nextLength > targetLength)
                    {
                        var desiredLength = targetLength - lengthSoFar;
                        var desiredPrecentOfNextLength = desiredLength / nextLength;
                        Point interpolatedPoint = GetInterpolatedPoint(_linePoints[j], _linePoints[j + 1], desiredPrecentOfNextLength);
                        AddSpacedPoint(interpolatedPoint);
                        break;
                    }

                    lengthSoFar += nextLength;
                }
            }

            AddSpacedPoint(_linePoints.Last());

            // reverse the frames if you want the wire to draw backwards
            _linePoints.Reverse();
        }
    }
}