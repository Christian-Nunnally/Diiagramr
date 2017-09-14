using PropertyChanged;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;
// ReSharper disable MemberCanBePrivate.Global Public setters used for deserialization

namespace Diiagramr.Model
{
    [DataContract]
    [AddINotifyPropertyChangedInterface]
    public class Wire : ModelBase
    {
        [DataMember]
        public OutputTerminal SourceTerminal { get; set; }

        [DataMember]
        public InputTerminal SinkTerminal { get; set; }

        [DataMember]
        public double X1 { get; set; }

        [DataMember]
        public double Y1 { get; set; }

        [DataMember]
        public double X2 { get; set; }

        [DataMember]
        public double Y2 { get; set; }

        private Wire() { }

        public Wire(OutputTerminal sourceTerminal, InputTerminal sinkTerminal)
        {
            if (!sourceTerminal.Type.IsSubclassOf(sinkTerminal.Type) && sourceTerminal.Type != sinkTerminal.Type) return;

            SinkTerminal = sinkTerminal;
            SourceTerminal = sourceTerminal;

            SetupPropertyChangedNotificationsFromTerminals();

            SourceTerminal.DisconnectWire();
            SinkTerminal.DisconnectWire();

            SourceTerminal.ConnectedWire = this;
            SinkTerminal.ConnectedWire = this;
        }

        private void ConnectedTerminalOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var terminal = (TerminalModel)sender;
            if (e.PropertyName.Equals("X"))
            {
                if (terminal == SinkTerminal) X1 = terminal.X;
                else if (terminal == SourceTerminal) X2 = terminal.X;
            }
            else if (e.PropertyName.Equals("Y"))
            {
                if (terminal == SinkTerminal) Y1 = terminal.Y;
                else if (terminal == SourceTerminal) Y2 = terminal.Y;
            }
            else if (e.PropertyName.Equals("Direction"))
            {

            }
            else if (e.PropertyName.Equals(nameof(TerminalModel.Data)))
            {
                if (terminal == SourceTerminal)
                {
                    SinkTerminal.Data = SourceTerminal.Data;
                }
            }
        }

        public void SetupPropertyChangedNotificationsFromTerminals()
        {
            SourceTerminal.PropertyChanged += ConnectedTerminalOnPropertyChanged;
            SinkTerminal.PropertyChanged += ConnectedTerminalOnPropertyChanged;

            SinkTerminal.Data = SourceTerminal.Data;
        }

        public void PretendWireMoved()
        {
            SourceTerminal.Wiggle();
            SinkTerminal.Wiggle();
            OnModelPropertyChanged(nameof(X1));
            OnModelPropertyChanged(nameof(Y1));
            OnModelPropertyChanged(nameof(X2));
            OnModelPropertyChanged(nameof(Y2));
        }
    }
}