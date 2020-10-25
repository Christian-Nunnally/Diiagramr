using DiiagramrModel;
using System;

namespace DiiagramrAPI.Editor.Diagrams
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public class InputTerminalAttribute : Attribute
    {
        public InputTerminalAttribute(Direction defaultDirection)
        {
            DefaultDirection = defaultDirection;
        }

        public Direction DefaultDirection { get; }
    }
}