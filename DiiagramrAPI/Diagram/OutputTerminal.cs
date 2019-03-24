using DiiagramrAPI.Diagram.Model;
using System;

namespace DiiagramrAPI.Diagram
{
    public class OutputTerminal : Terminal
    {
        public OutputTerminal(TerminalModel outputTerminal) : base(outputTerminal)
        {
            if (outputTerminal.Kind != TerminalKind.Output)
            {
                throw new ArgumentException("Terminal must be output kind for OutputTerminalViewModel");
            }
        }
    }
}
