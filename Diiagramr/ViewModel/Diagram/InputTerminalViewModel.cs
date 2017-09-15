﻿using System;
using Diiagramr.Model;

namespace Diiagramr.ViewModel.Diagram
{
    public class InputTerminalViewModel : TerminalViewModel
    {

        public InputTerminalViewModel(TerminalModel inputTerminal) : base(inputTerminal)
        {
            if (inputTerminal.Kind != TerminalKind.Input) throw new ArgumentException("Terminal must be input kind for InputTerminalViewModel");
        }

        public sealed override void WireToTerminal(TerminalModel terminal)
        {
            if (terminal.Kind != TerminalKind.Output) return;
            base.WireToTerminal(terminal);
        }
    }
}