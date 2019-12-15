using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;

namespace DiiagramrFadeCandy
{
    public class SpectrumEffectNode : Node
    {
        public SpectrumEffectNode()
        {
            Width = 30;
            Height = 30;
            Name = "Spectrum Visualizer Effect";
        }

        public SpectrumEffect SpectrumEffect { get; set; } = new SpectrumEffect { Color = new Color(255f, 255f, 255f, 255f) };

        [OutputTerminal(nameof(Effect), Direction.South)]
        public GraphicEffect Effect { get; set; }

        [InputTerminal("Color", Direction.West)]
        public void SetColor(Color data)
        {
            if (data != null)
            {
                SpectrumEffect.Color = data;
                Output(SpectrumEffect, nameof(Effect));
            }
        }

        [InputTerminal("Signal", Direction.North)]
        public void SignalChanged(byte[] data)
        {
            SpectrumEffect.SpectrumData = data;
            Output(SpectrumEffect, nameof(Effect));
        }
    }
}