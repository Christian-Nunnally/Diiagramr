using DiiagramrModel;
using System;

namespace DiiagramrAPI.Editor.Diagrams
{
    [AttributeUsage(AttributeTargets.Property)]
    public class OutputTerminalAttribute : TerminalAttribute
    {
        public OutputTerminalAttribute(Direction defaultDirection) : base(defaultDirection)
        {
        }
    }
}