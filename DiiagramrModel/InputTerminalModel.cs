namespace DiiagramrModel
{
    using System;
    using System.Runtime.Serialization;

    [DataContract(IsReference = true)]
    public class InputTerminalModel : TerminalModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InputTerminalModel"/> class.
        /// </summary>
        /// <param name="name">The user visible name of the terminal.</param>
        /// <param name="type">The data type of the terminal.</param>
        /// <param name="defaultSide">The default side of a node the terminal belongs on.</param>
        /// <param name="index">The unique index of the terminal.</param>
        public InputTerminalModel(string name, Type type, Direction defaultDirection, int index) 
            : base(name, type, defaultDirection, index)
        {
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
            if (otherTerminal is InputTerminalModel)
            {
                throw new ModelValidationException(this, "Connect this terminal to an output instead of an input");
            }

            wire.SinkTerminal = this;
            wire.SourceTerminal = otherTerminal;
            otherTerminal.ConnectedWires.Add(wire);
            ConnectedWires.Add(wire);
        }
    }
}
