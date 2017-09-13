using Diiagramr.Model;
using Stylet;

namespace Diiagramr.ViewModel.Diagram
{
    public class OutputTerminalViewModel : TerminalViewModel, IViewAware
    {

        public OutputTerminalViewModel(OutputTerminal outputTerminal) : base(outputTerminal)
        {
        }

        public OutputTerminal OutputTerminal => Terminal as OutputTerminal;

        public sealed override void WireToTerminal(TerminalModel terminal)
        {
            if (!(terminal is InputTerminal)) return;
            base.WireToTerminal(terminal);
        }

        public sealed override void DropObject(object o)
        {
            var terminal = o as InputTerminal;
            if (terminal == null) return;
            WireFromTerminal(terminal);
        }
    }
}