using DiiagramrAPI.Diagram;

namespace DiiagramrFadeCandy
{
    public class SpectrumEffectNodeViewModel : Node
    {
        public SpectrumEffect SpectrumEffect { get; set; } = new SpectrumEffect();

        protected override void SetupNode(NodeSetup setup)
        {
            setup.NodeName("Spectrum Visualizer Effect");
            setup.NodeSize(30, 30);

            setup.InputTerminal<byte[]>("Signal", Direction.North).DataChanged += SignalInputChanged;
            setup.OutputTerminal<GraphicEffect>("Effect", Direction.South).Data = SpectrumEffect;

            var colorInputTerminal = setup.InputTerminal<Color>("Color", Direction.West);
            colorInputTerminal.DataChanged += ColorInputTerminalDataChanged;
            colorInputTerminal.Data = new Color(255f, 255f, 255f, 255f);
        }

        private void ColorInputTerminalDataChanged(Color data)
        {
            if (data != null)
            {
                SpectrumEffect.Color = data;
            }
        }

        private void SignalInputChanged(byte[] data)
        {
            SpectrumEffect.SpectrumData = data;
        }
    }
}
