using PropertyChanged;
using System;
using System.Runtime.Serialization;

namespace Diiagramr.Model
{
    [DataContract(IsReference = true)]
    [AddINotifyPropertyChangedInterface]
    public class OutputTerminal : Terminal
    {
        private OutputTerminal() { }

        public OutputTerminal(string name, Type type) : base(name, type)
        {
        }
    }
}