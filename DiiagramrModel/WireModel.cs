namespace DiiagramrModel
{
    using DiiagramrCore;
    using DiiagramrModel2;
    using PropertyChanged;
    using System;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    /// <summary>
    /// A wire represents a link from one <see cref="TerminalModel"/> to another.
    /// </summary>
    [DataContract]
    [AddINotifyPropertyChangedInterface]
    public class WireModel : ModelBase
    {
        private TerminalModel _sinkTerminal;
        private TerminalModel _sourceTerminal;

        /// <summary>
        /// Initializes a new instance of the <see cref="WireModel"/> class.
        /// </summary>
        /// <remarks>
        /// Use <see cref="TerminalModel.ConnectWire(WireModel, TerminalModel)"/> to attach it to the model.
        /// </remarks>
        public WireModel()
        {
            PropertyChanged += PropertyChangedHandler;
        }

        [DataMember]
        public bool IsBroken { get; set; }

        /// <summary>
        /// Gets or sets the terminal that gets data from the <see cref="SourceTerminal"/>.
        /// </summary>
        [DataMember]
        public TerminalModel SinkTerminal
        {
            get => _sinkTerminal;
            set => _sinkTerminal.UpdateListeningProperty(value, () => _sinkTerminal = value, SinkTerminalOnPropertyChanged);
        }

        /// <summary>
        /// Gets or sets the terminal that provides data for the <see cref="SinkTerminal"/>.
        /// </summary>
        [DataMember]
        public TerminalModel SourceTerminal
        {
            get => _sourceTerminal;
            set => _sourceTerminal.UpdateListeningProperty(value, () => _sourceTerminal = value, SourceTerminalOnPropertyChanged);
        }

        /// <summary>
        /// Gets or sets the x position of the beginning of the wire.
        /// </summary>
        [DataMember]
        public virtual double X1 { get; set; }

        /// <summary>
        /// Gets or sets the y position of the beginning of the wire.
        /// </summary>
        [DataMember]
        public virtual double Y1 { get; set; }

        /// <summary>
        /// Gets or sets the x position of the end of the wire.
        /// </summary>
        [DataMember]
        public virtual double X2 { get; set; }

        /// <summary>
        /// Gets or sets the y position of the end of the wire.
        /// </summary>
        [DataMember]
        public virtual double Y2 { get; set; }

        public void PropagateData()
        {
            if (IsBroken || SourceTerminal == null || SinkTerminal == null)
            {
                return;
            }
            else
            {
                PropagateDataCore();
            }
        }

        private void PropagateDataCore()
        {
            object newData;
            if (SourceTerminal.Type == typeof(object))
            {
                if (SourceTerminal.Data == null)
                {
                    newData = null;
                }
                else
                {
                    var fromType = SourceTerminal.Data.GetType();
                    var toType = SinkTerminal.Type;
                    if (!TryCoerseValue(fromType, toType, SourceTerminal?.Data, out newData))
                    {
                        IsBroken = true;
                    }
                }
            }
            else
            {
                var fromType = SourceTerminal.Type;
                var toType = SinkTerminal.Type;
                if (!TryCoerseValue(fromType, toType, SourceTerminal?.Data, out newData))
                {
                    IsBroken = true;
                }
            }
            SinkTerminal.Data = newData;
        }

        private bool TryCoerseValue(Type fromType, Type toType, object value, out object coersedValue)
        {
            if (toType.IsAssignableFrom(fromType))
            {
                coersedValue = value;
                return true;
            }
            if (ValueCoersionHelper.CanCoerseValue(fromType, toType))
            {
                coersedValue = ValueCoersionHelper.CoerseValue(fromType, toType, SourceTerminal?.Data);
                return true;
            }
            coersedValue = null;
            return false;
        }

        private void PropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SourceTerminal))
            {
                UpdateSourceTerminal();
            }
            else if (e.PropertyName == nameof(SinkTerminal))
            {
                UpdateSinkTerminal();
            }
        }

        private void UpdateSourceTerminal()
        {
            if (SourceTerminal is object)
            {
                X1 = SourceTerminal.X;
                Y1 = SourceTerminal.Y;
            }
        }

        private void UpdateSinkTerminal()
        {
            if (SinkTerminal is object)
            {
                PropagateData();
                X2 = SinkTerminal.X;
                Y2 = SinkTerminal.Y;
            }
        }

        private void SinkTerminalOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var terminal = (TerminalModel)sender;
            if (e.PropertyName.Equals(nameof(TerminalModel.X)))
            {
                X2 = terminal.X;
            }
            else if (e.PropertyName.Equals(nameof(TerminalModel.Y)))
            {
                Y2 = terminal.Y;
            }
        }

        private void SourceTerminalOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var terminal = (TerminalModel)sender;
            if (e.PropertyName.Equals(nameof(TerminalModel.X)))
            {
                X1 = terminal.X;
            }
            else if (e.PropertyName.Equals(nameof(TerminalModel.Y)))
            {
                Y1 = terminal.Y;
            }
        }
    }
}