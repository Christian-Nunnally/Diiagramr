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
        public void SetWidth(int width)
        {
            CellularAutomataEffect.Width = width;
        }

        [InputTerminal(Direction.East)]
        public void SetHeight(int height)
        {
            CellularAutomataEffect.Height = height;
        }

        [InputTerminal(Direction.West)]
        public void Randomize(bool _)
        {
            CellularAutomataEffect.Randomize();
        }
    }
}