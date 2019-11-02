using DiiagramrAPI.Editor;
using DiiagramrAPI.Shell.Commands;
using System;
using System.Windows;

namespace DiiagramrAPI.Commands
{
    public class ResizeNodeCommand : ICommand
    {
        private readonly Size _size;

        public ResizeNodeCommand(Size size)
        {
            _size = size;
        }

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