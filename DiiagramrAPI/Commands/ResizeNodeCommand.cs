using DiiagramrAPI.Application.Commands;
using DiiagramrAPI.Editor.Diagrams;
using System;
using System.Windows;

namespace DiiagramrAPI.Commands
{
    /// <summary>
    /// An undoable command that resizes a single node.
    /// </summary>
    public class ResizeNodeCommand : IReversableCommand
    {
        private readonly Size _size;

        /// <summary>
        /// Creates a new instance of <see cref="ResizeNodeCommand"/>
        /// </summary>
        /// <param name="size">The size to set a node to.</param>
        public ResizeNodeCommand(Size size)
        {
            _size = size;
        }

        /// <inheritdoc/>
        public Action Execute(object parameter)
        {
            if (parameter is Node node)
            {
                var oldWidth = node.Width;
                var oldHeight = node.Height;
                node.Width = _size.Width;
                node.Height = _size.Height;
                return () =>
                {
                    node.Width = oldWidth;
                    node.Height = oldHeight;
                };
            }

            return () => { };
        }
    }
}