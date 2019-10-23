using DiiagramrAPI.Service;
using PropertyChanged;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace DiiagramrAPI.Diagram.Model
{
    [DataContract]
    [AddINotifyPropertyChangedInterface]
    public class WireModel : ModelBase
    {
        private TerminalModel _sinkTerminal;
        private TerminalModel _sourceTerminal;

        public WireModel()
        {
            PropertyChanged += PropertyChangedHandler;
        }

        private void PropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SourceTerminal) || e.PropertyName == nameof(SinkTerminal))
            {
                if (SinkTerminal is object)
                {
                    SinkTerminal.Data = SourceTerminal?.Data;
                    X1 = SinkTerminal.X;
                    Y1 = SinkTerminal.Y;
                }
                if (SourceTerminal is object)
                {
                    X2 = SourceTerminal.X;
                    Y2 = SourceTerminal.Y;
                }
            }
        }

        [DataMember]
        public TerminalModel SinkTerminal
        {
            get => _sinkTerminal;
            set => _sinkTerminal.UpdateListeningProperty(value, () => _sinkTerminal = value, SinkTerminalOnPropertyChanged);
        }

        [DataMember]
        public TerminalModel SourceTerminal
        {
            get => _sourceTerminal;
            set => _sourceTerminal.UpdateListeningProperty(value, () => _sourceTerminal = value, SourceTerminalOnPropertyChanged);
        }

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

        public virtual void ResetWire()
        {
            SinkTerminal.Data = null;
        }

        private void SinkTerminalOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(TerminalModel.X)))
            {
                X1 = ((TerminalModel)sender).X;
            }
            else if (e.PropertyName.Equals(nameof(TerminalModel.Y)))
            {
                Y1 = ((TerminalModel)sender).Y;
            }
        }

        private void SourceTerminalOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(TerminalModel.X)))
            {
                X2 = ((TerminalModel)sender).X;
            }
            else if (e.PropertyName.Equals(nameof(TerminalModel.Y)))
            {
                Y2 = ((TerminalModel)sender).Y;
            }
        }
    }
}
