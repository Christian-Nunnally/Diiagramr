using DiiagramrAPI.Application.Commands;
using DiiagramrAPI.Editor.Diagrams;
using System;
using System.Windows;

namespace DiiagramrAPI.Commands
{
    /// <summary>
    /// An undoable command that moves a single node on a diagram.
    /// </summary>
    public class MoveNodeCommand : IReversableCommand
    {
        private readonly Point _point;

        /// <summary>
        /// Creates a new instance of <see cref="MoveNodeCommand"/>.
        /// </summary>
        /// <param name="point">The point to move the node to.</param>
        public MoveNodeCommand(Point point)
        {
            _point = point;
        }

        /// <inheritdoc/>
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