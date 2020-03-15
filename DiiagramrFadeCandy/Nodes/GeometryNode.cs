using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

namespace DiiagramrFadeCandy.Nodes
{
    internal class GeometryNode : Node
    {
        private RawVector2 _previousPoint = new RawVector2(0, 0);
        private bool _openFigure = false;

        public GeometryNode()
        {
            var xamlPath = "M14,3.23V5.29C16.89,6.15 19,8.83 19,12C19,15.17 16.89,17.84 14,18.7V20.77C18,19.86 21,16.28 21,12C21,7.72 18,4.14 14,3.23M16.5,12C16.5,10.23 15.5,8.71 14,7.97V16C15.5,15.29 16.5,13.76 16.5,12M3,9V15H7L12,20V4L7,9H3Z";
            PathGeometry = GetGeometryPathFromXamlPathMarkup(xamlPath);
        }

        [OutputTerminal(Direction.North)]
        public PathGeometry PathGeometry { get; set; }

        private PathGeometry GetGeometryPathFromXamlPathMarkup(string xamlPathMarkup)
        {
            var pathGeometry = new PathGeometry(LedMatrixNode.d2dFactory);
            var geometrySink = pathGeometry.Open();
            geometrySink.SetFillMode(xamlPathMarkup.StartsWith("F1") ? FillMode.Winding : FillMode.Alternate);
            var figureDescriptions = xamlPathMarkup.StartsWith("F") ? xamlPathMarkup.Substring(2) : xamlPathMarkup;

            FillGeometryFromFigureDescriptions(figureDescriptions, geometrySink);

            geometrySink.Close();
            return pathGeometry;
        }

        private void FillGeometryFromFigureDescriptions(string figureDescriptions, GeometrySink geometrySink)
        {
            if (figureDescriptions.Length == 0)
            {
                return;
            }
            var nextFigureDescriptions = ProcessFigureDescriptionAndReturnRemainingDescriptions(figureDescriptions, geometrySink);
            FillGeometryFromFigureDescriptions(nextFigureDescriptions, geometrySink);
        }

        private string ProcessFigureDescriptionAndReturnRemainingDescriptions(string figureDescriptions, GeometrySink geometrySink)
        {
            return figureDescriptions[0] switch
            {
                'M' => StartFigureAbsolute(figureDescriptions, geometrySink),
                'm' => StartFigureRelative(figureDescriptions, geometrySink),
                'L' => DrawLineAbsolute(figureDescriptions, geometrySink),
                'l' => DrawLineRelative(figureDescriptions, geometrySink),
                'H' => DrawHorizontalLineAbsolute(figureDescriptions, geometrySink),
                'h' => DrawHorizontalLineRelative(figureDescriptions, geometrySink),
                'V' => DrawVerticalLineAbsolute(figureDescriptions, geometrySink),
                'v' => DrawVerticalLineRelative(figureDescriptions, geometrySink),
                'C' => DrawCubicBezierCurveAbsolute(figureDescriptions, geometrySink),
                'c' => DrawCubicBezierCurveAbsolute(figureDescriptions, geometrySink),
                'Q' => DrawQuadraticBezierCurveAbsolute(figureDescriptions, geometrySink),
                'q' => DrawQuadraticBezierCurveAbsolute(figureDescriptions, geometrySink),
                'S' => DrawSmoothCubicBezierCurveAbsolute(figureDescriptions, geometrySink),
                's' => DrawSmoothCubicBezierCurveAbsolute(figureDescriptions, geometrySink),
                'T' => DrawSmoothQuadraticBezierCurveAbsolute(figureDescriptions, geometrySink),
                't' => DrawSmoothQuadraticBezierCurveAbsolute(figureDescriptions, geometrySink),
                'A' => DrawArcAbsolute(figureDescriptions, geometrySink),
                'a' => DrawArcAbsolute(figureDescriptions, geometrySink),
                'Z' => EndFigure(geometrySink),
                'z' => EndFigure(geometrySink),
                _ => string.Empty,
            };
        }

        private string EndFigure(GeometrySink geometrySink)
        {
            geometrySink.EndFigure(FigureEnd.Closed);
            _openFigure = false;
            return string.Empty;
        }

        private string DrawArcAbsolute(string drawCommands, GeometrySink geometrySink)
        {
            var size = GetPointAndAdvanceString(drawCommands, out var drawCommands2);
            var rotationAngle = GetSingleFloatAndAdvanceString(drawCommands2, out var drawCommands3);
            var isLargeArcFlag = GetSingleFloatAndAdvanceString(drawCommands3, out var drawCommands4);
            var sweepDirectionFlag = GetSingleFloatAndAdvanceString(drawCommands4, out var drawCommands5);
            var endPoint = GetPointAndAdvanceString(drawCommands5, out var figureDescriptions);
            var arcSegment = new ArcSegment();
            arcSegment.Size = new SharpDX.Size2F(size.X, size.Y);
            arcSegment.RotationAngle = rotationAngle;
            arcSegment.ArcSize = isLargeArcFlag > 0.5 ? ArcSize.Large : ArcSize.Small;
            arcSegment.SweepDirection = sweepDirectionFlag > 0.5 ? SweepDirection.Clockwise : SweepDirection.CounterClockwise;
            arcSegment.Point = endPoint;
            geometrySink.AddArc(arcSegment);
            _previousPoint = endPoint;
            return figureDescriptions;
        }

        private string DrawSmoothQuadraticBezierCurveAbsolute(string drawCommands, GeometrySink geometrySink)
        {
            // TODO: Make this actually follow documentation https://docs.microsoft.com/en-us/dotnet/framework/wpf/graphics-multimedia/path-markup-syntax?view=netframework-4.8#drawcommands
            var controlPoint = GetPointAndAdvanceString(drawCommands, out var drawCommands2);
            var endPoint = GetPointAndAdvanceString(drawCommands2, out var figureDescriptions);
            var bezierSegment = new QuadraticBezierSegment();
            bezierSegment.Point1 = controlPoint;
            bezierSegment.Point2 = endPoint;
            geometrySink.AddQuadraticBezier(bezierSegment);
            _previousPoint = endPoint;
            return figureDescriptions;
        }

        private string DrawSmoothCubicBezierCurveAbsolute(string drawCommands, GeometrySink geometrySink)
        {
            var controlPoint1 = _previousPoint; // TODO: Make this actually follow documentation https://docs.microsoft.com/en-us/dotnet/framework/wpf/graphics-multimedia/path-markup-syntax?view=netframework-4.8#drawcommands
            var controlPoint2 = GetPointAndAdvanceString(drawCommands, out var drawCommands2);
            var endPoint = GetPointAndAdvanceString(drawCommands2, out var figureDescriptions);
            var bezierSegment = new BezierSegment();
            bezierSegment.Point1 = controlPoint1;
            bezierSegment.Point2 = controlPoint2;
            bezierSegment.Point3 = endPoint;
            geometrySink.AddBezier(bezierSegment);
            _previousPoint = endPoint;
            return figureDescriptions;
        }

        private string DrawQuadraticBezierCurveAbsolute(string drawCommands, GeometrySink geometrySink)
        {
            var controlPoint = GetPointAndAdvanceString(drawCommands, out var drawCommands2);
            var endPoint = GetPointAndAdvanceString(drawCommands2, out var figureDescriptions);
            var bezierSegment = new QuadraticBezierSegment();
            bezierSegment.Point1 = controlPoint;
            bezierSegment.Point2 = endPoint;
            geometrySink.AddQuadraticBezier(bezierSegment);
            _previousPoint = endPoint;
            return figureDescriptions;
        }

        private string DrawCubicBezierCurveAbsolute(string drawCommands, GeometrySink geometrySink)
        {
            var controlPoint1 = GetPointAndAdvanceString(drawCommands, out var drawCommands2);
            var controlPoint2 = GetPointAndAdvanceString(drawCommands2, out var drawCommands3);
            var endPoint = GetPointAndAdvanceString(drawCommands3, out var figureDescriptions);
            var bezierSegment = new BezierSegment();
            bezierSegment.Point1 = controlPoint1;
            bezierSegment.Point2 = controlPoint2;
            bezierSegment.Point3 = endPoint;
            geometrySink.AddBezier(bezierSegment);
            _previousPoint = endPoint;
            return figureDescriptions;
        }

        private string DrawVerticalLineAbsolute(string drawCommands, GeometrySink geometrySink)
        {
            var endY = GetSingleFloatAndAdvanceString(drawCommands, out var figureDescriptions);
            AddLine(geometrySink, _previousPoint.X, endY);
            return figureDescriptions;
        }

        private void AddLine(GeometrySink geometrySink, float endX, float endY) => AddLine(geometrySink, new RawVector2(endX, endY));

        private void AddLine(GeometrySink geometrySink, RawVector2 point)
        {
            geometrySink.AddLine(point);
            _previousPoint = point;
        }

        private string DrawVerticalLineRelative(string drawCommands, GeometrySink geometrySink)
        {
            var endY = GetSingleFloatAndAdvanceString(drawCommands, out var figureDescriptions);
            AddLine(geometrySink, _previousPoint.X, endY + _previousPoint.Y);
            return figureDescriptions;
        }

        private string DrawHorizontalLineAbsolute(string drawCommands, GeometrySink geometrySink)
        {
            var endX = GetSingleFloatAndAdvanceString(drawCommands, out var figureDescriptions);
            AddLine(geometrySink, endX, _previousPoint.Y);
            return figureDescriptions;
        }

        private string DrawHorizontalLineRelative(string drawCommands, GeometrySink geometrySink)
        {
            var endX = GetSingleFloatAndAdvanceString(drawCommands, out var figureDescriptions);
            AddLine(geometrySink, endX + _previousPoint.X, _previousPoint.Y);
            return figureDescriptions;
        }

        private string DrawLineRelative(string drawCommands, GeometrySink geometrySink)
        {
            var relativePoint = GetPointAndAdvanceString(drawCommands, out var figureDescriptions);
            AddLine(geometrySink, relativePoint.X + _previousPoint.X, relativePoint.Y + _previousPoint.Y);
            return figureDescriptions;
        }

        private string DrawLineAbsolute(string drawCommands, GeometrySink geometrySink)
        {
            var endPoint = GetPointAndAdvanceString(drawCommands, out var figureDescriptions);
            AddLine(geometrySink, endPoint);
            return figureDescriptions;
        }

        private string StartFigureAbsolute(string drawCommands, GeometrySink geometrySink)
        {
            if (_openFigure)
            {
                geometrySink.EndFigure(FigureEnd.Closed);
            }
            var startPoint = GetPointAndAdvanceString(drawCommands, out var figureDescriptions);
            geometrySink.BeginFigure(startPoint, FigureBegin.Filled);
            _openFigure = true;
            _previousPoint = startPoint;
            return figureDescriptions;
        }

        private string StartFigureRelative(string drawCommands, GeometrySink geometrySink)
        {
            if (_openFigure)
            {
                geometrySink.EndFigure(FigureEnd.Open);
            }
            var relativePoint = GetPointAndAdvanceString(drawCommands, out var figureDescriptions);
            var newPoint = new RawVector2(relativePoint.X + _previousPoint.X, relativePoint.Y + _previousPoint.Y);
            geometrySink.BeginFigure(newPoint, FigureBegin.Filled);
            _openFigure = true;
            _previousPoint = newPoint;
            return figureDescriptions;
        }

        private float GetSingleFloatAndAdvanceString(string drawCommands, out string resultingString)
        {
            if (!IsNumeric(drawCommands[0])) drawCommands = drawCommands.Substring(1);
            int i = 0;
            while (i < drawCommands.Length && IsNumeric(drawCommands[i])) i++;
            resultingString = drawCommands.Substring(i);
            return float.Parse(drawCommands.Substring(0, i));
        }

        private RawVector2 GetPointAndAdvanceString(string drawCommands, out string resultingString)
        {
            if (!IsNumeric(drawCommands[0])) drawCommands = drawCommands.Substring(1);
            var commaIndex = drawCommands.IndexOf(',');
            int i = commaIndex + 1;
            while (i < drawCommands.Length && IsNumeric(drawCommands[i])) i++;
            var startX = drawCommands.Substring(0, commaIndex);
            var startY = drawCommands.Substring(commaIndex + 1, i - commaIndex - 1);
            resultingString = drawCommands.Substring(i);
            var nextPoint = new RawVector2(float.Parse(startX), float.Parse(startY));
            return nextPoint;
        }

        private bool IsNumeric(char character) =>
            character == '.'
            || character == '1'
            || character == '2'
            || character == '3'
            || character == '4'
            || character == '5'
            || character == '6'
            || character == '7'
            || character == '8'
            || character == '9'
            || character == '0'
            || character == '-';
    }
}