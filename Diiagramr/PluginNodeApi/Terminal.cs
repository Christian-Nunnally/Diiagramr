using System;
using System.ComponentModel;
using Diiagramr.ViewModel.Diagram;

namespace Diiagramr.PluginNodeApi
{
    public delegate void TerminalDataChangedDelegate<in T>(T data);

    /// <summary>
    ///     Generic wrapper around a <see cref="TerminalViewModel" /> for <see cref="PluginNode" /> clients to get and set
    ///     terminal data.
    ///     Setting the data on this terminal notifies the underlying <see cref="TerminalViewModel" /> so that the data can be
    ///     propagated
    ///     by any connected <see cref="WireViewModel" />.  When the underlying <see cref="TerminalViewModel" /> has its data
    ///     set, the value
    ///     is casted to the correct type before being given to the <see cref="PluginNode" /> client for consumption.
    /// </summary>
    /// <typeparam name="T">The datatype of the terminal.</typeparam>
    public class Terminal<T>
    {
        private readonly TerminalViewModel _underlyingTerminal;
        private T _data;

        public Terminal(TerminalViewModel underlyingTerminal)
        {
            _underlyingTerminal = underlyingTerminal ?? throw new ArgumentNullException(nameof(underlyingTerminal));
            _underlyingTerminal.PropertyChanged += UnderlyingTerminalOnPropertyChanged;
        }

        /// <summary>
        /// The data on the terminal. Setting this will result in data propagating to connected wires.
        /// When data is set, <see cref="DataChanged"/> will be invoked with the new value as the argument.
        /// </summary>
        public T Data
        {
            get => _data;
            set
            {
                if (_data.Equals(value)) return;
                _underlyingTerminal.Data = value;
                _data = value;
                DataChanged?.Invoke(_data);
            }
        }

        /// <summary>
        /// Notifies subscribers when <see cref="Data"/> is changed.
        /// </summary>
        public event TerminalDataChangedDelegate<T> DataChanged;

        private void UnderlyingTerminalOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!e.PropertyName.Equals(nameof(TerminalViewModel.Data))) return;
            Data = (T) _underlyingTerminal.Data;
        }
    }
}