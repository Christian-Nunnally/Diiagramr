using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;

namespace DiiagramrFadeCandy
{
    public class SpectrumEffectNode : Node
    {
        public SpectrumEffectNode()
        {
            Width = 60;
            Height = 60;
            Name = "Spectrum Visualizer Effect";
        }

        public SpectrumEffect SpectrumEffect => Effect as SpectrumEffect;

        [NodeSetting]
        [OutputTerminal(Direction.South)]
        public GraphicEffect Effect { get; set; } = new SpectrumEffect { Color = new Color(255f, 255f, 255f, 255f) };

        [NodeSetting]
        [InputTerminal(Direction.West)]
        public float MaxValueDecayRate
        {
            get => SpectrumEffect.MaxValueDecayRate;
            set => SpectrumEffect.MaxValueDecayRate = value;
        }

        [NodeSetting]
        [InputTerminal(Direction.West)]
        public Color Color
        {
            get => SpectrumEffect.Color ?? new Color(255f, 255f, 255f, 255f);
            set => SpectrumEffect.Color = value;
        }

        [NodeSetting]
        [InputTerminal(Direction.North)]
        public float[] Data
        {
            get => SpectrumEffect.SpectrumData;
            set => SpectrumEffect.SpectrumData = value;
        }

        [NodeSetting]
        [InputTerminal(Direction.East)]
        public float BarWidthScale
        {
            get => SpectrumEffect.BarWidthScale;
            set => SpectrumEffect.BarWidthScale = value;
        }

        [NodeSetting]
        [InputTerminal(Direction.East)]
        public float ScaleExponent
        {
            get => SpectrumEffect.ScaleExponent;
            set => SpectrumEffect.ScaleExponent = value;
        }

        [NodeSetting]
        [InputTerminal(Direction.East)]
        public float BarHeight
        {
            get => SpectrumEffect.BarHeight;
            set => SpectrumEffect.BarHeight = value;
        }

        [NodeSetting]
        [InputTerminal(Direction.West)]
        public int IterationGrowthRate
        {
            get => SpectrumEffect.IterationGrowthRate;
            set => SpectrumEffect.IterationGrowthRate = value;
        }

        [NodeSetting]
        [InputTerminal(Direction.North)]
        public bool SpectrographMode
        {
            get => SpectrumEffect.SpectrographMode;
            set
            {
                SpectrumEffect.SpectrographMode = value;
            }
        }
    }
}