using Diiagramr.Model;
using System.Collections.Generic;

namespace Diiagramr.ViewModel.Diagram
{
    public delegate IDictionary<OutputTerminal, object> InputTerminalDelegate(object arg);

    public class DelegateMapper
    {
        private Dictionary<int, InputTerminalDelegate> InputTerminalDelegates { get; set; }

        public DelegateMapper()
        {
            InputTerminalDelegates = new Dictionary<int, InputTerminalDelegate>();
        }

        public void AddMapping(int index, InputTerminalDelegate inputDelegate)
        {
            InputTerminalDelegates.Add(index, inputDelegate);
        }

        public IDictionary<OutputTerminal, object> Invoke(int terminalIndex, object arg)
        {
            return InputTerminalDelegates.ContainsKey(terminalIndex) ? InputTerminalDelegates[terminalIndex].Invoke(arg) : null;
        }
    }
}