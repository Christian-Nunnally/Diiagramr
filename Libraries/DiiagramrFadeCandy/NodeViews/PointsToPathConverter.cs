using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace DiiagramrFadeCandy
{
    [ValueConversion(typeof(Point[]), typeof(Geometry))]
    public class PointsToPathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var points = (Point[])value;
            if ((points?.Length ?? 0) <= 0)
            {
                return new List<LineSegment>();
            }

            PathFigure figure = CreateFigureFromPoints(points);
            return CreateGeometryWithFigure(figure);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        private static PathFigure CreateFigureFromPoints(Point[] points)
        {
            var segments = new List<LineSegment>();
            for (var i = 1; i < points.Length; i++)
            {
                segments.Add(new LineSegment(points[i], true));
            }
            var figure = new PathFigure(points[0], segments, false);
            return figure;
        }

        private static PathGeometry CreateGeometryWithFigure(PathFigure figure)
        {
            var geometry = new PathGeometry();
            geometry.Figures.Add(figure);
            return geometry;
        }
    }
}