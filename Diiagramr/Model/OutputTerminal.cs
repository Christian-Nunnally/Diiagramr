using PropertyChanged;
using System;
using System.Runtime.Serialization;

namespace Diiagramr.Model
{
    [DataContract(IsReference = true)]
    [AddINotifyPropertyChangedInterface]
    public class OutputTerminal : TerminalModel
    {
        private OutputTerminal() { }

        public OutputTerminal(string name, Type type, int terminalIndex) : base(name, type, terminalIndex)
        {
        }
    }
}