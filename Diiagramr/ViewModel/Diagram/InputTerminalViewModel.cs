using Diiagramr.Model;
using Stylet;

// ReSharper disable UnusedMember.Global

namespace Diiagramr.ViewModel.Diagram
{
    public class InputTerminalViewModel : TerminalViewModel, IViewAware
    {

        public InputTerminalViewModel(InputTerminal inputTerminal) : base(inputTerminal)
        {
        }

        public sealed override void WireToTerminal(Terminal terminal)
        {
            if (!(terminal is OutputTerminal)) return;
            base.WireToTerminal(terminal);
        }
    }
}