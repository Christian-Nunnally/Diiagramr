using PropertyChanged;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace DiiagramrAPI.Diagram.Model
{
    [DataContract]
    [AddINotifyPropertyChangedInterface]
    public class WireModel : ModelBase
    {
        private bool _isActive = true;

        public event Action WireDataChanged;

        public WireModel(TerminalModel startTerminal, TerminalModel endTerminal)
        {
            if (startTerminal.Kind == endTerminal.Kind)
            {
                throw new ArgumentException("Wires require one input terminal and one output terminal");
            }

            SinkTerminal = startTerminal.Kind == TerminalKind.Input ? startTerminal : endTerminal;
            SourceTerminal = startTerminal.Kind == TerminalKind.Output ? startTerminal : endTerminal;
            SourceTerminal.ConnectWire(this);
            SinkTerminal.ConnectWire(this);

            SetupTerminalPropertyChangeNotifications();
            UserWiredFromInput = startTerminal == SourceTerminal;
            SinkTerminal.Data = SourceTerminal.Data;
        }

        [DataMember]
        public TerminalModel SinkTerminal { get; set; }

        [DataMember]
        public TerminalModel SourceTerminal { get; set; }

        [IgnoreDataMember]
        public bool UserWiredFromInput { get; }

        [DataMember]
        public virtual double X1 { get; set; }

        [DataMember]
        public virtual double X2 { get; set; }

        [DataMember]
        public virtual double Y1 { get; set; }

        [DataMember]
        public virtual double Y2 { get; set; }

        public virtual void DisableWire()
        {
            _isActive = false;
        }

        public virtual void DisconnectWire()
        {
            SourceTerminal.PropertyChanged -= SourceTerminalOnPropertyChanged;
            SinkTerminal.PropertyChanged -= SinkTerminalOnPropertyChanged;
            SourceTerminal.DisconnectWire(this);
            SinkTerminal.Data = null;
            SinkTerminal.DisconnectWire(this);
            SourceTerminal = null;
            SinkTerminal = null;
        }

        public virtual void EnableWire()
        {
            _isActive = true;
            SinkTerminal.Data = SourceTerminal.Data;
        }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
            SetupTerminalPropertyChangeNotifications();
            SinkTerminal.Data = SourceTerminal.Data;
        }

        public virtual void ResetWire()
        {
            SinkTerminal.Data = null;
        }

        private void SetupTerminalPropertyChangeNotifications()
        {
            SourceTerminal.PropertyChanged += SourceTerminalOnPropertyChanged;
            SinkTerminal.PropertyChanged += SinkTerminalOnPropertyChanged;

            X1 = SinkTerminal.X;
            Y1 = SinkTerminal.Y;
            X2 = SourceTerminal.X;
            Y2 = SourceTerminal.Y;
        }

        private void SinkTerminalOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var sink = (TerminalModel)sender;
            if (e.PropertyName.Equals(nameof(TerminalModel.X)))
            {
                X1 = sink.X;
            }
            else if (e.PropertyName.Equals(nameof(TerminalModel.Y)))
            {
                Y1 = sink.Y;
            }
        }

        private void SourceTerminalOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var source = (TerminalModel)sender;
            if (_isActive)
            {
                if (e.PropertyName.Equals(nameof(TerminalModel.Data)))
                {
                    SinkTerminal.Data = source.Data;
                    WireDataChanged();
                    return;
                }
            }

            if (e.PropertyName.Equals(nameof(TerminalModel.X)))
            {
                X2 = source.X;
            }
            else if (e.PropertyName.Equals(nameof(TerminalModel.Y)))
            {
                Y2 = source.Y;
            }
        }
    }
}
