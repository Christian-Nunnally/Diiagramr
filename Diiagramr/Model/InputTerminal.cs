using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Diiagramr.Model
{
    [DataContract(IsReference = true)]
    [AddINotifyPropertyChangedInterface]
    public class InputTerminal : Terminal
    {
        [DataMember]
        public int TerminalIndex { get; set; }

        [DataMember]
        public DiagramNode OwningNode { get; set; }

        private InputTerminal() { }

        public InputTerminal(string name, Type type, DiagramNode owningNode, int terminalIndex) : base(name, type)
        {
            OwningNode = owningNode;
            TerminalIndex = terminalIndex;
        }

        public IDictionary<OutputTerminal, object> Execute(object arg)
        {
            return OwningNode.NodeViewModel.InvokeInput(TerminalIndex, arg);
        }
    }
}