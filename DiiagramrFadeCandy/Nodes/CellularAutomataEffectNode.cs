using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;

namespace DiiagramrFadeCandy
{
    public class CellularAutomataEffectNode : Node
    {
        public CellularAutomataEffectNode()
        {
            Width = 30;
            Height = 30;
            Name = "Cellular Automata Effect";
        }

        public CellularAutomataEffect CellularAutomataEffect => Effect as CellularAutomataEffect;

        [OutputTerminal(Direction.South)]
        public GraphicEffect Effect { get; set; } = new CellularAutomataEffect();

        [InputTerminal(Direction.East)]
        public int Width
        {
            get => CellularAutomataEffect.Width;
            set => CellularAutomataEffect.Width = value;
        }

        [InputTerminal(Direction.East)]
        public int Height
        {
            get => CellularAutomataEffect.Height;
            set => CellularAutomataEffect.Height = value;
        }

        [InputTerminal(Direction.West)]
        public bool Randomize
        {
            set => CellularAutomataEffect.Randomize();
            get => true;
        }
    }
}