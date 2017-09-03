using System;
using System.Collections.Generic;
using Diiagramr.Model;

namespace Diiagramr.ViewModel.Diagram
{
    public delegate IDictionary<OutputTerminal, object> InputTerminalDelegate(object arg);

    public class DelegateMapper
    {
        private Dictionary<string, InputTerminalDelegate> InputTerminalDelegates { get; set; }

        public DelegateMapper()
        {
            InputTerminalDelegates = new Dictionary<string, InputTerminalDelegate>();
        }

        public void AddMapping(string key, InputTerminalDelegate inputDelegate)
        {
            InputTerminalDelegates.Add(key, inputDelegate);
        }

        public IDictionary<OutputTerminal, object> Invoke(string delegateKey, object arg)
        {
            if (string.IsNullOrWhiteSpace(delegateKey)) return null;
            return InputTerminalDelegates.ContainsKey(delegateKey) ? InputTerminalDelegates[delegateKey].Invoke(arg) : null;
        }
    }
}