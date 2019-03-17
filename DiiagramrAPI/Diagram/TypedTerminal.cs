using DiiagramrAPI.Diagram;
using System;
using System.ComponentModel;

namespace DiiagramrAPI.Diagram
{
    public delegate void TerminalDataChangedDelegate<in T>(T data);

    /// <summary>
    /// Generic wrapper around a <see cref="Terminal" /> for <see cref="Terminal" /> clients to get and set
    /// terminal data. Setting the data on this terminal notifies the underlying <see cref="Wire" /> 
    /// so that the data can be propagated by any connected <see cref="Terminal" />.  When the underlying 
    /// <see cref="Node" /> has its data set, the value is casted to the correct type before being given 
    /// to the <see cref="Node" /> client for consumption.   
    /// </summary>
    /// <typeparam name="T">The datatype of the terminal.</typeparam>
    public class TypedTerminal<T>
    {
        private T _data;

        public TypedTerminal(Terminal underlyingTerminal)
        {
            UnderlyingTerminal = underlyingTerminal ?? throw new ArgumentNullException(nameof(underlyingTerminal));
            UnderlyingTerminal.PropertyChanged += UnderlyingTerminalOnPropertyChanged;
            Data = (T)(underlyingTerminal.Data ?? default(T));
        }

        /// <summary>
        ///     Notifies subscribers when <see cref="Data" /> is changed.
        /// </summary>
        public event TerminalDataChangedDelegate<T> DataChanged
        {
            add
            {
                _dataChanged += value;
                _dataChanged.Invoke(_data);
            }
            remove
            {
                _dataChanged -= value;
            }
        }

        private event TerminalDataChangedDelegate<T> _dataChanged;

        /// <summary>
        ///     The data on the terminal. Setting this will result in data propagating to connected wires.
        ///     When data is set, <see cref="DataChanged" /> will be invoked with the new value as the argument.
        /// </summary>
        public T Data
        {
            get => _data;

            set
            {
                if (_data != null && _data.Equals(value))
                {
                    return;
                }

                UnderlyingTerminal.Data = value;
                _data = value;
                _dataChanged?.Invoke(_data);
            }
        }

        public Terminal UnderlyingTerminal { get; }

        public void ChangeTerminalData(object data)
        {
            Data = (T)(data ?? default(T));
        }

        private void CastAndSetData(object data)
        {
            if (data == null)
            {
                Data = default(T);
            }
            else
            {
                try
                {
                    Data = (T)data;
                }
                catch (InvalidCastException)
                {
                    Data = default(T);
                }
            }
        }

        private void UnderlyingTerminalOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!e.PropertyName.Equals(nameof(Terminal.Data)))
            {
                return;
            }

            CastAndSetData(UnderlyingTerminal.Data);
        }
    }
}
