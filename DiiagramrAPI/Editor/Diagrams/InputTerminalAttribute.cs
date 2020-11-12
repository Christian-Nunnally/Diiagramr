using DiiagramrModel;
using System;

namespace DiiagramrAPI.Editor.Diagrams
{
    [AttributeUsage(AttributeTargets.Property)]
    public class InputTerminalAttribute : TerminalAttribute
    {
        public InputTerminalAttribute(Direction defaultDirection, bool isCoalescing = false) : base(defaultDirection)
        {
            IsCoalescing = isCoalescing;
        }

        public bool IsCoalescing { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public abstract class TerminalAttribute : Attribute
    {
        public TerminalAttribute(Direction defaultDirection)
        {
            DefaultDirection = defaultDirection;
        }

        public Direction DefaultDirection { get; }
    }
}