﻿namespace DiiagramrModel
{
    using PropertyChanged;
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// A terminal that can only be wired to an <see cref="InputTerminal"/>.
    /// </summary>
    [DataContract(IsReference = true)]
    [KnownType(typeof(OutputTerminalModel))]
    public class OutputTerminalModel : TerminalModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OutputTerminalModel"/> class.
        /// </summary>
        /// <param name="name">The user visible name of the terminal.</param>
        /// <param name="type">The data type of the terminal.</param>
        /// <param name="defaultSide">The default side of a node the terminal belongs on.</param>
        public OutputTerminalModel(string name, Type type, Direction defaultSide)
            : base(name, type, defaultSide)
        {
            SerializeableTypes.Add(GetType());
        }

        /// <inheritdoc/>
        [DoNotCheckEquality]
        public override object Data
        {
            get => base.Data;
            set
            {
                base.Data = value;
                PropagateDataToAllWires();
            }
        }

        /// <summary>
        /// Connects this terminal to another terminal via a wire.
        /// </summary>
        /// <param name="wire">The wire to connect the terminals with.</param>
        /// <param name="otherTerminal">The terminal to wire to.</param>
        public override void ConnectWire(WireModel wire, TerminalModel otherTerminal)
        {
            if (ConnectedWires.Contains(wire))
            {
                throw new ModelValidationException(this, "Remove this wire from a terminal before connecting it again");
            }

            if (otherTerminal is OutputTerminalModel)
            {
                throw new ModelValidationException(this, "Connect this terminal to an input instead of an output");
            }

            wire.SinkTerminal = otherTerminal;
            wire.SourceTerminal = this;
            otherTerminal.ConnectedWires.Add(wire);
            ConnectedWires.Add(wire);
            PropagateDataToAllWires();
        }
    }
}