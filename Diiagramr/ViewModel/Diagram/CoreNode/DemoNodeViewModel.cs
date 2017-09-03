using Diiagramr.Model;

namespace Diiagramr.ViewModel.Diagram
{
    public class DemoNodeViewModel : AbstractNodeViewModel
    {
        public override string Name => "Demo Node";

        public override void ConstructTerminals()
        {
            ConstructNewInputTerminal("Input", typeof(int), Direction.None, "");
            ConstructNewOutputTerminal("Output", typeof(int), Direction.None);
        }
    }
}