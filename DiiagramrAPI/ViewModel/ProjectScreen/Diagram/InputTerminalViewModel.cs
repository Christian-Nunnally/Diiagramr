using DiiagramrAPI.Model;
using DiiagramrAPI.Service;
using DiiagramrAPI.ViewModel.ProjectScreen.Diagram;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DiiagramrAPI.ViewModel.Diagram
{
    public class InputTerminalViewModel : TerminalViewModel
    {
        private string _directEditTextBoxText = "0";

        public TextBox DirectEditTextBox { get; set; }

        public InputTerminalViewModel(TerminalModel inputTerminal) : base(inputTerminal)
        {
            if (inputTerminal.Kind != TerminalKind.Input)
            {
                throw new ArgumentException("Terminal must be input kind for InputTerminalViewModel");
            }
        }

        public string DirectEditTextBoxText
        {
            get => _directEditTextBoxText;

            set
            {
                if (IsIntType)
                {
                    if (int.TryParse(value, out int parseResult))
                    {
                        _directEditTextBoxText = value;
                        Data = parseResult;
                    }
                }
                else if (IsFloatType)
                {
                    if (float.TryParse(value, out float parseResult))
                    {
                        _directEditTextBoxText = value;
                        Data = parseResult;
                    }
                }
                else if (IsStringType)
                {
                    _directEditTextBoxText = value;
                    Data = value;
                }
                else if (IsCharType)
                {
                    if (char.TryParse(value, out char parseResult))
                    {
                        _directEditTextBoxText = value;
                        Data = parseResult;
                    }
                }
            }
        }

        public bool IsBoolType => TerminalModel.Type == typeof(bool);
        public bool IsCharType => TerminalModel.Type == typeof(char);
        public bool IsDirectEditTextBoxVisible => IsDirectlyEditableType && IsSelected && !IsConnected;
        public bool IsDirectlyEditableType => IsIntType || IsFloatType || IsStringType || IsCharType;
        public bool IsFloatType => TerminalModel.Type == typeof(float);
        public bool IsIntType => TerminalModel.Type == typeof(int);
        public bool IsStringType => TerminalModel.Type == typeof(string);

        public void DirectEditTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            TextBox inputTextBox = (TextBox)e.OriginalSource;
            DirectEditTextBoxText = Data?.ToString();
            inputTextBox.Dispatcher.BeginInvoke(
                new Action(delegate
                {
                    inputTextBox.SelectAll();
                }), System.Windows.Threading.DispatcherPriority.ApplicationIdle);
        }

        public void DirectEditTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                IsSelected = false;
            }
        }

        public void DirectEditTextBoxVisibiliyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            TextBox inputTextBox = (TextBox)sender;

            DirectEditTextBoxText = Data?.ToString();
        }

        public void TerminalDoubleClicked()
        {
            if (IsBoolType)
            {
                Data = !(bool)(Data ?? false);
            }
        }

        public sealed override bool WireToTerminal(TerminalModel terminal)
        {
            if (terminal.Kind != TerminalKind.Output)
            {
                return false;
            }

            return base.WireToTerminal(terminal);
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == nameof(IsSelected))
            {
                OnPropertyChanged(nameof(IsDirectEditTextBoxVisible));
            }
            else if (propertyName == nameof(IsDirectEditTextBoxVisible))
            {
                if (IsDirectEditTextBoxVisible)
                {
                    SetTerminalAdorner(new DirectEditTextBoxAdorner(View, this));
                }
                else
                {
                    Adorner = null;
                }
            }
        }
    }
}
