using DiiagramrModel;
using System;

namespace DiiagramrAPI.Editor.Diagrams
{
    [AttributeUsage(AttributeTargets.Method)]
    public class InputTerminalAttribute : Attribute
    {
        public InputTerminalAttribute(string terminalName, Direction defaultDirection)
        {
            TerminalName = terminalName;
            DefaultDirection = defaultDirection;
        }

        public string TerminalName { get; }
        public Direction DefaultDirection { get; }
    }
}