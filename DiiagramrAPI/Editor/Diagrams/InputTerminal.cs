using DiiagramrModel;
using System.Reflection;
using System.Windows.Controls;

namespace DiiagramrAPI.Editor.Diagrams
{
    public class InputTerminal : Terminal
    {
        private string _directEditTextBoxText = "0";

        public InputTerminal(InputTerminalModel inputTerminal)
            : base(inputTerminal)
        {
        }

        public MethodInfo InputHandlerMethodInfo { get; set; }

        public TextBox DirectEditTextBox { get; set; }

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

        public void TerminalDoubleClicked()
        {
            if (IsBoolType)
            {
                Data = !(bool)(Data ?? false);
            }
        }
    }
}