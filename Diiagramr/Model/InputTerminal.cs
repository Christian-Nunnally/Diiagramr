using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Diiagramr.Model
{
    [DataContract(IsReference = true)]
    [AddINotifyPropertyChangedInterface]
    public class InputTerminal : TerminalModel
    {
        [DataMember]
        public DiagramNode OwningNode { get; set; }

        private InputTerminal() { }

        public InputTerminal(string name, Type type, DiagramNode owningNode, int terminalIndex) : base(name, type, terminalIndex)
        {
            OwningNode = owningNode;
        }
    }
}