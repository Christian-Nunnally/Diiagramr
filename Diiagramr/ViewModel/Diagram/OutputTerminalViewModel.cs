using System;
using Diiagramr.Model;

namespace Diiagramr.ViewModel.Diagram
{
    public class OutputTerminalViewModel : TerminalViewModel
    {

        public OutputTerminalViewModel(TerminalModel outputTerminal) : base(outputTerminal)
        {
            if (outputTerminal.Kind != TerminalKind.Output) throw new ArgumentException("Terminal must be output kind for OutputTerminalViewModel");
        }

        public sealed override void WireToTerminal(TerminalModel terminal)
        {
            if (terminal.Kind != TerminalKind.Input) return;
            base.WireToTerminal(terminal);
        }
    }
}