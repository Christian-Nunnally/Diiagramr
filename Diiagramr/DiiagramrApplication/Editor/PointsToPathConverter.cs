﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace DiiagramrApplication.Editor
{
    /// <summary>
    /// Converts a list of point to a path object.
    /// </summary>
    [ValueConversion(typeof(Point[]), typeof(Geometry))]
    public class PointsToPathConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var points = (Point[])value;
            if (points == null)
            {
                return new List<LineSegment>();
            }

            if (points.Length <= 0)
            {
                return null;
            }

            var start = points[0];
            var segments = new List<LineSegment>();
            for (var i = 1; i < points.Length; i++)
            {
                segments.Add(new LineSegment(points[i], true));
            }
            var figure = new PathFigure(start, segments, false);
            var geometry = new PathGeometry();
            geometry.Figures.Add(figure);
            return geometry;
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}