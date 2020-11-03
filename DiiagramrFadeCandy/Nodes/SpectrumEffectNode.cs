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
            Name = "Array Visualizer Effect";
        }

        public SpectrumEffect SpectrumEffect => Effect as SpectrumEffect;

        [OutputTerminal(Direction.South)]
        public GraphicEffect Effect { get; set; } = new SpectrumEffect { Color = new Color(255f, 255f, 255f, 255f) };

        [InputTerminal(Direction.West)]
        public void Color(Color data)
        {
            if (data != null)
            {
                SpectrumEffect.Color = data;
            }
        }

        [InputTerminal(Direction.North)]
        public void Array(float[] data)
        {
            SpectrumEffect.SpectrumData = data;
        }

        [InputTerminal(Direction.East)]
        public void BarWidth(float barWidth)
        {
            SpectrumEffect.BarWidthScale = barWidth;
        }

        [InputTerminal(Direction.East)]
        public void ScaleExponent(float scaleExponent)
        {
            SpectrumEffect.ScaleExponent = scaleExponent;
        }

        [InputTerminal(Direction.East)]
        public void SpetrographMode(bool spectrographMode)
        {
            SpectrumEffect.SpectrographMode = spectrographMode;
        }
    }
}