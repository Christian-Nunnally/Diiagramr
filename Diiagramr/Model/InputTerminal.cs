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
        public string DelegateKey { get; set; }

        [DataMember]
        public DiagramNode OwningNode { get; set; }

        private InputTerminal() { }

        public InputTerminal(string name, Type type, DiagramNode owningNode) : base(name, type)
        {
            OwningNode = owningNode;
        }

        public void SetInputTerminalDelegate(string delegateKey)
        {
            DelegateKey = delegateKey;
        }

        public IDictionary<OutputTerminal, object> Execute(object arg)
        {
            return OwningNode.NodeViewModel?.DelegateMapper.Invoke(DelegateKey, arg);
        }
    }
}