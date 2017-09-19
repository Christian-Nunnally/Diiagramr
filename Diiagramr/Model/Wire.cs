using PropertyChanged;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Diiagramr.Model
{
    [DataContract]
    [AddINotifyPropertyChangedInterface]
    public class Wire : ModelBase
    {
        [DataMember]
        public TerminalModel SourceTerminal { get; set; }

        [DataMember]
        public TerminalModel SinkTerminal { get; set; }

        [DataMember]
        public bool IsActive { get; set; }

        [DataMember]
        public double X1 { get; set; }

        [DataMember]
        public double Y1 { get; set; }

        [DataMember]
        public double X2 { get; set; }

        [DataMember]
        public double Y2 { get; set; }

        private Wire() { }

        public Wire(TerminalModel terminal1, TerminalModel terminal2)
        {
            IsActive = true;
            SinkTerminal = terminal1.Kind == TerminalKind.Input ? terminal1 : terminal2;
            SourceTerminal = terminal1.Kind == TerminalKind.Output ? terminal1 : terminal2; ;

            if (!SourceTerminal.Type.IsSubclassOf(SinkTerminal.Type) && SourceTerminal.Type != SinkTerminal.Type) return;

            SourceTerminal.DisconnectWire();
            SinkTerminal.DisconnectWire();

            SourceTerminal.ConnectedWire = this;
            SinkTerminal.ConnectedWire = this;
        }

        private void ConnectedTerminalOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Debug.WriteLine("ConnectedTerminalOnPropertyChanged "+IsActive);
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
            else if (!IsActive) return;
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
            Debug.WriteLine("SetupPropertyChangedNotificationsFromTerminals");
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