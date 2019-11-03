using DiiagramrAPI.Application.Commands;
using DiiagramrAPI.Editor.Diagrams;
using System;
using System.Windows;

namespace DiiagramrAPI.Commands
{
    public class MoveNodeCommand : ICommand
    {
        private readonly Point _point;

        public MoveNodeCommand(Point point)
        {
            _point = point;
        }

        public Action Execute(object parameter)
        {
            if (parameter is Node node)
            {
                var oldX = node.X;
                var oldY = node.Y;
                node.X = _point.X;
                node.Y = _point.Y;
                return () =>
                {
                    node.X = oldX;
                    node.Y = oldY;
                };
            }
            return () => { };
        }
    }
}