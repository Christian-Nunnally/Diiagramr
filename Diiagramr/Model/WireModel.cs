﻿using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using PropertyChanged;

namespace Diiagramr.Model
{
    [DataContract]
    [AddINotifyPropertyChangedInterface]
    public class WireModel : ModelBase
    {
        private WireModel() { }

        public WireModel(TerminalModel terminal1, TerminalModel terminal2)
        {
            if (terminal1 == null) throw new ArgumentNullException(nameof(terminal1));
            if (terminal2 == null) throw new ArgumentNullException(nameof(terminal2));
            if (terminal1.Kind == terminal2.Kind) throw new ArgumentException("Wires require one input terminal and one output terminal");

            SinkTerminal = terminal1.Kind == TerminalKind.Input ? terminal1 : terminal2;
            SourceTerminal = terminal1.Kind == TerminalKind.Output ? terminal1 : terminal2;

            if (!SourceTerminal.Type.IsSubclassOf(SinkTerminal.Type) && SourceTerminal.Type != SinkTerminal.Type) return;

            SourceTerminal.DisconnectWire();
            SinkTerminal.DisconnectWire();

            SourceTerminal.ConnectedWire = this;
            SinkTerminal.ConnectedWire = this;
            SetupTerminalPropertyChangeNotifications();
        }

        [DataMember]
        public TerminalModel SourceTerminal { get; set; }

        [DataMember]
        public TerminalModel SinkTerminal { get; set; }

        [DataMember]
        public double X1 { get; set; }

        [DataMember]
        public double Y1 { get; set; }

        [DataMember]
        public double X2 { get; set; }

        [DataMember]
        public double Y2 { get; set; }

        private void SourceTerminalOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var source = (TerminalModel) sender;
            if (e.PropertyName.Equals(nameof(TerminalModel.X))) X2 = source.X;
            else if (e.PropertyName.Equals(nameof(TerminalModel.Y))) Y2 = source.Y;
            else if (e.PropertyName.Equals(nameof(TerminalModel.Data))) SinkTerminal.Data = source.Data;
        }

        private void SinkTerminalOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var sink = (TerminalModel) sender;
            if (e.PropertyName.Equals(nameof(TerminalModel.X))) X1 = sink.X;
            else if (e.PropertyName.Equals(nameof(TerminalModel.Y))) Y1 = sink.Y;
        }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
            SetupTerminalPropertyChangeNotifications();
        }

        private void SetupTerminalPropertyChangeNotifications()
        {
            SourceTerminal.PropertyChanged += SourceTerminalOnPropertyChanged;
            SinkTerminal.PropertyChanged += SinkTerminalOnPropertyChanged;

            X1 = SinkTerminal.X;
            Y1 = SinkTerminal.Y;
            X2 = SourceTerminal.X;
            Y2 = SourceTerminal.Y;

            SinkTerminal.Data = SourceTerminal.Data;
        }
    }
}