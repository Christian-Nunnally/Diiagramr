using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DiiagramrAPI.Model;
using DiiagramrAPI.ViewModel.ProjectScreen.Diagram;

namespace DiiagramrAPI.ViewModel.Diagram
{
    public class InputTerminalViewModel : TerminalViewModel
    {
        private string _intTextBoxText = "0";

        public bool IsIntType => TerminalModel.Type == typeof(int);
        public bool IsIntTextBoxVisible => IsIntType && IsSelected && !IsConnected;

        public string IntTextBoxText
        {
            get => _intTextBoxText;
            set
            {
                if (int.TryParse(value, out int outInt))
                {
                    _intTextBoxText = value;
                    if (IsIntType)
                    {
                        Data = outInt;
                    }
                }
            }
        }

        public InputTerminalViewModel(TerminalModel inputTerminal) : base(inputTerminal)
        {
            if (inputTerminal.Kind != TerminalKind.Input) throw new ArgumentException("Terminal must be input kind for InputTerminalViewModel");
        }

        public sealed override bool WireToTerminal(TerminalModel terminal)
        {
            if (terminal.Kind != TerminalKind.Output) return false;
            return base.WireToTerminal(terminal);
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName.Equals(nameof(IsSelected))) OnPropertyChanged(nameof(IsIntTextBoxVisible));
        }

        public void IntTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) IsSelected = false;
        }

        public void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)e.OriginalSource;
            tb.Dispatcher.BeginInvoke(
                new Action(delegate
                {
                    tb.SelectAll();
                }), System.Windows.Threading.DispatcherPriority.ContextIdle);
        }
    }
}