using DiiagramrModel;
using System;

namespace DiiagramrAPI.Editor.Diagrams
{
    [AttributeUsage(AttributeTargets.Property)]
    public class OutputTerminalAttribute : Attribute
    {
        public OutputTerminalAttribute(string terminalName, Direction defaultDirection)
        {
            TerminalName = terminalName;
            DefaultDirection = defaultDirection;
        }

        public string TerminalName { get; }
        public Direction DefaultDirection { get; }
    }
}