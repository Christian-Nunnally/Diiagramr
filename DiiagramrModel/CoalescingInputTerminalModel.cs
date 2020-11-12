namespace DiiagramrModel
{
    using DiiagramrCore;
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract(IsReference = true)]
    [KnownType(typeof(CoalescingInputTerminalModel))]
    public class CoalescingInputTerminalModel : InputTerminalModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CoalescingInputTerminalModel"/> class.
        /// </summary>
        /// <param name="name">The user visible name of the terminal.</param>
        /// <param name="type">The data type of the terminal.</param>
        /// <param name="defaultDirection">The default side of a node the terminal belongs on.</param>
        public CoalescingInputTerminalModel(string name, Type type, Direction defaultDirection)
            : base(name, type, defaultDirection)
        {
            SerializeableTypes.Add(GetType());
        }

        [DataMember]
        public Dictionary<WireModel, int> WireToIndexMap { get; set; } = new Dictionary<WireModel, int>();

        [DataMember]
        public List<object> DataList { get; set; } = new List<object>();

        public override object Data
        {
            get => DataList?.ConvertToList((Type ?? typeof(object))) ?? new List<object>();

            set
            {
            }
        }

        public override void DisconnectWire(WireModel wire, TerminalModel otherTerminal)
        {
            var disconnectedWireIndex = WireToIndexMap[wire];
            DataList.RemoveAt(disconnectedWireIndex);
            WireToIndexMap.Clear();
            base.DisconnectWire(wire, otherTerminal);
            foreach (var eachWire in ConnectedWires)
            {
                WireToIndexMap.Add(eachWire, WireToIndexMap.Count);
            }
            InvokeDataChanged(Data);
        }

        public override void ConnectWire(WireModel wire, TerminalModel otherTerminal)
        {
            if (!WireToIndexMap.ContainsKey(wire))
            {
                DataList.Add(null);
                WireToIndexMap.Add(wire, WireToIndexMap.Count);
            }
            base.ConnectWire(wire, otherTerminal);
            InvokeDataChanged(Data);
        }

        public override void SetDataFromWire(object data, WireModel wire)
        {
            if (WireToIndexMap.TryGetValue(wire, out var index))
            {
                DataList[index] = data;
            }
            else
            {
                DataList.Add(data);
                WireToIndexMap.Add(wire, WireToIndexMap.Count);
            }
            InvokeDataChanged(Data);
        }
    }
}