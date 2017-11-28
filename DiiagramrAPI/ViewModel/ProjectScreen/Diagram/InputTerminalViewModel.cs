using System;
using DiiagramrAPI.Model;
using DiiagramrAPI.ViewModel.ProjectScreen.Diagram;

namespace DiiagramrAPI.ViewModel.Diagram
{
    public class InputTerminalViewModel : TerminalViewModel
    {
        public InputTerminalViewModel(TerminalModel inputTerminal) : base(inputTerminal)
        {
            if (inputTerminal.Kind != TerminalKind.Input) throw new ArgumentException("Terminal must be input kind for InputTerminalViewModel");
        }

        public sealed override bool WireToTerminal(TerminalModel terminal)
        {
            if (terminal.Kind != TerminalKind.Output) return false;
            return base.WireToTerminal(terminal);
        }
    }
}