using DiiagramrModel;

namespace DiiagramrAPI.Editor.Diagrams
{
    public class InputTerminal : Terminal
    {
        public InputTerminal(InputTerminalModel inputTerminal)
            : base(inputTerminal)
        {
        }

        public void TerminalDoubleClicked()
        {
            if (Model.Type == typeof(bool))
            {
                Data = !(bool)(Data ?? false);
            }
        }
    }
}