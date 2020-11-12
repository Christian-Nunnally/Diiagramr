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
        public float MaxValueDecayRate
        {
            get => SpectrumEffect.MaxValueDecayRate;
            set => SpectrumEffect.MaxValueDecayRate = value;
        }

        [InputTerminal(Direction.West)]
        public Color Color
        {
            get => SpectrumEffect.Color ?? new Color(255f, 255f, 255f, 255f);
            set => SpectrumEffect.Color = value;
        }

        [InputTerminal(Direction.North)]
        public float[] Data
        {
            get => SpectrumEffect.SpectrumData;
            set => SpectrumEffect.SpectrumData = value;
        }

        [InputTerminal(Direction.East)]
        public float BarWidthScale
        {
            get => SpectrumEffect.BarWidthScale;
            set => SpectrumEffect.BarWidthScale = value;
        }

        [InputTerminal(Direction.East)]
        public float ScaleExponent
        {
            get => SpectrumEffect.ScaleExponent;
            set => SpectrumEffect.ScaleExponent = value;
        }

        [InputTerminal(Direction.North)]
        public bool SpectrographMode
        {
            get => SpectrumEffect.SpectrographMode;
            set => SpectrumEffect.SpectrographMode = value;
        }
    }
}