using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Diiagramr.Model;
using Diiagramr.View;
using Stylet;

namespace Diiagramr.ViewModel.Diagram
{
    public class WireViewModel : Screen
    {
        private const double UTurnLength = 50.0;
        private const double WireDistanceOutOfTerminal = 25.0;

        public WireViewModel(Wire wire)
        {
            Wire = wire;
            wire.PropertyChanged += WireOnPropertyChanged;
            
            wire.PretendWireMoved();
            ConfigureWirePoints();
        }

        public Point[] Points { get; set; }

        public double X1 { get; set; }
        public double X2 { get; set; }
        public double Y1 { get; set; }
        public double Y2 { get; set; }

        public Wire Wire { get; set; }

        private void WireOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("X1")) X1 = Wire.X1 + DiagramConstants.NodeBorderWidth;
            if (e.PropertyName.Equals("X2")) X2 = Wire.X2 + DiagramConstants.NodeBorderWidth;
            if (e.PropertyName.Equals("Y1")) Y1 = Wire.Y1 + DiagramConstants.NodeBorderWidth;
            if (e.PropertyName.Equals("Y2")) Y2 = Wire.Y2 + DiagramConstants.NodeBorderWidth;

            if (e.PropertyName.Equals("SourceTerminal") || e.PropertyName.Equals("SinkTerminal"))
            {
                Wire.PropertyChanged -= WireOnPropertyChanged;
                return;
            }

            ConfigureWirePoints();
        }

        private void ConfigureWirePoints()
        {
            var start = new Point(X1, Y1);
            var end = new Point(X2, Y2);
            var stubStart = AdjustPointInDirection(start, Wire.SinkTerminal.Direction, WireDistanceOutOfTerminal);
            var stubEnd = AdjustPointInDirection(end, Wire.SourceTerminal.Direction, WireDistanceOutOfTerminal);

            var points = new List<Point>();
            points.Add(start);
            var bannedDirectionForStart = OppositeDirection(Wire.SinkTerminal.Direction);
            var bannedDirectionForEnd = OppositeDirection(Wire.SourceTerminal.Direction);
            WireTwoPoints(stubStart, stubEnd, bannedDirectionForStart, bannedDirectionForEnd, points, 0);
            points.Add(end);

            Points = points.ToArray();
        }

        private Point AdjustPointInDirection(Point p, Direction direction, double amount)
        {
            if (direction == Direction.North) return new Point(p.X, p.Y - amount);
            if (direction == Direction.South) return new Point(p.X, p.Y + amount);
            return direction == Direction.East ? new Point(p.X + amount, p.Y) : new Point(p.X - amount, p.Y);
        }

        public void WireMouseDown(object sender, MouseEventArgs e)
        {
            DisconnectWire();
        }

        public void DisconnectWire()
        {
            Wire.SourceTerminal?.DisconnectWire();
            Wire.SinkTerminal?.DisconnectWire();
        }

        private IList<Point> WireTwoPoints(Point start, Point end, Direction bannedDirectionForStart, Direction bannedDirectionForEnd, IList<Point> pointsSoFar, int uturnCount)
        {
            pointsSoFar.Add(start);
            switch (bannedDirectionForStart)
            {
                case Direction.South:
                    // The end is above the start
                    if (start.Y >= end.Y)
                    {
                        if (Math.Abs(start.Y - end.Y) < 0.5 || Math.Abs(start.X - end.X) < 0.5)
                        {
                            pointsSoFar.Add(end);
                            return pointsSoFar;
                        }

                        if (bannedDirectionForEnd == Direction.East)
                            if (start.X > end.X) return WireHorizontiallyTowardsEnd(start, end, bannedDirectionForEnd, pointsSoFar, uturnCount);
                            else return WireVerticallyTowardsEnd(start, end, bannedDirectionForEnd, pointsSoFar, uturnCount);

                        if (bannedDirectionForEnd == Direction.West)
                            if (start.X > end.X) WireVerticallyTowardsEnd(start, end, bannedDirectionForEnd, pointsSoFar, uturnCount);
                            else return WireHorizontiallyTowardsEnd(start, end, bannedDirectionForEnd, pointsSoFar, uturnCount);
                        return WireVerticallyTowardsEnd(start, end, bannedDirectionForEnd, pointsSoFar, uturnCount);
                    }
                    break;
                case Direction.West:
                    // The end is to the left of the start
                    if (start.X <= end.X)
                    {
                        if (Math.Abs(start.Y - end.Y) < 0.5 || Math.Abs(start.X - end.X) < 0.5)
                        {
                            pointsSoFar.Add(end);
                            return pointsSoFar;
                        }

                        if (bannedDirectionForEnd == Direction.North)
                            if (start.Y > end.Y) return WireHorizontiallyTowardsEnd(start, end, bannedDirectionForEnd, pointsSoFar, uturnCount);
                            else return WireVerticallyTowardsEnd(start, end, bannedDirectionForEnd, pointsSoFar, uturnCount);

                        if (bannedDirectionForEnd == Direction.South)
                            if (start.Y > end.Y) WireVerticallyTowardsEnd(start, end, bannedDirectionForEnd, pointsSoFar, uturnCount);
                            else return WireHorizontiallyTowardsEnd(start, end, bannedDirectionForEnd, pointsSoFar, uturnCount);
                        return WireHorizontiallyTowardsEnd(start, end, bannedDirectionForEnd, pointsSoFar, uturnCount);
                    }
                    break;
                case Direction.North:
                    // The end is below the start
                    if (start.Y <= end.Y)
                    {
                        if (Math.Abs(start.Y - end.Y) < 0.5 || Math.Abs(start.X - end.X) < 0.5)
                        {
                            pointsSoFar.Add(end);
                            return pointsSoFar;
                        }

                        if (bannedDirectionForEnd == Direction.East)
                            if (start.X > end.X) return WireHorizontiallyTowardsEnd(start, end, bannedDirectionForEnd, pointsSoFar, uturnCount);
                            else return WireVerticallyTowardsEnd(start, end, bannedDirectionForEnd, pointsSoFar, uturnCount);

                        if (bannedDirectionForEnd == Direction.West)
                            if (start.X > end.X) WireVerticallyTowardsEnd(start, end, bannedDirectionForEnd, pointsSoFar, uturnCount);
                            else return WireHorizontiallyTowardsEnd(start, end, bannedDirectionForEnd, pointsSoFar, uturnCount);
                        return WireVerticallyTowardsEnd(start, end, bannedDirectionForEnd, pointsSoFar, uturnCount);
                    }
                    break;
                case Direction.East:
                    // The end is to the right of the start
                    if (start.X >= end.X)
                    {
                        if (Math.Abs(start.Y - end.Y) < 0.5 || Math.Abs(start.X - end.X) < 0.5)
                        {
                            pointsSoFar.Add(end);
                            return pointsSoFar;
                        }

                        if (bannedDirectionForEnd == Direction.North)
                            if (start.Y > end.Y) return WireHorizontiallyTowardsEnd(start, end, bannedDirectionForEnd, pointsSoFar, uturnCount);
                            else return WireVerticallyTowardsEnd(start, end, bannedDirectionForEnd, pointsSoFar, uturnCount);

                        if (bannedDirectionForEnd == Direction.South)
                            if (start.Y > end.Y) WireVerticallyTowardsEnd(start, end, bannedDirectionForEnd, pointsSoFar, uturnCount);
                            else return WireHorizontiallyTowardsEnd(start, end, bannedDirectionForEnd, pointsSoFar, uturnCount);
                        return WireHorizontiallyTowardsEnd(start, end, bannedDirectionForEnd, pointsSoFar, uturnCount);
                    }
                    break;
            }

            // UTurn if nothing else works
            return UTurn(start, end, bannedDirectionForStart, bannedDirectionForEnd, pointsSoFar, uturnCount);
        }

        private IList<Point> UTurn(Point start, Point end, Direction bannedDirectionForStart, Direction bannedDirectionForEnd, IList<Point> pointsSoFar, int uturnCount)
        {
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
                    newPoint = new Point(start.X + UTurnLength, start.Y);
                    newBannedDirection = Direction.West;
                }
                else
                {
                    newPoint = new Point(start.X - UTurnLength, start.Y);
                    newBannedDirection = Direction.East;
                }
            }
            else
            {
                if (start.Y < end.Y)
                {
                    newPoint = new Point(start.X, start.Y + UTurnLength);
                    newBannedDirection = Direction.North;
                }
                else
                {
                    newPoint = new Point(start.X, start.Y - UTurnLength);
                    newBannedDirection = Direction.South;
                }
            }

            return WireTwoPoints(newPoint, end, newBannedDirection, bannedDirectionForEnd, pointsSoFar, uturnCount);
        }

        private Direction OppositeDirection(Direction direction)
        {
            if (direction == Direction.North) return Direction.South;
            if (direction == Direction.South) return Direction.North;
            if (direction == Direction.East) return Direction.West;
            return Direction.East;
        }

        private IList<Point> WireHorizontiallyTowardsEnd(Point start, Point end, Direction bannedDirectionForEnd, IList<Point> pointsSoFar, int uturnCount)
        {
            var bannedStart = start.X < end.X ? Direction.West : Direction.East;
            var newPoint = new Point(end.X, start.Y);
            return WireTwoPoints(newPoint, end, bannedStart, bannedDirectionForEnd, pointsSoFar, uturnCount);
        }

        private IList<Point> WireVerticallyTowardsEnd(Point start, Point end, Direction bannedDirectionForEnd, IList<Point> pointsSoFar, int uturnCount)
        {
            var bannedStart = start.Y < end.Y ? Direction.North : Direction.South;
            var newPoint = new Point(start.X, end.Y);
            return WireTwoPoints(newPoint, end, bannedStart, bannedDirectionForEnd, pointsSoFar, uturnCount);
        }
    }
}