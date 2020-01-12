using DiiagramrModel;
using System;

namespace DiiagramrAPI.Editor.Diagrams
{
    [AttributeUsage(AttributeTargets.Property)]
    public class OutputTerminalAttribute : Attribute
    {
        public OutputTerminalAttribute(Direction defaultDirection)
        {
            DefaultDirection = defaultDirection;
        }

        public Direction DefaultDirection { get; }
    }
}