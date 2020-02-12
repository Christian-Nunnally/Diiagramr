using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;

namespace DiiagramrFadeCandy
{
    public class KaleidoscopeEffectNode : Node
    {
        public KaleidoscopeEffectNode()
        {
            Width = 30;
            Height = 30;
            Name = "Kaleidoscope Effect";
        }

        public KaleidoscopeEffect KaleidoscopeEffect => Effect as KaleidoscopeEffect;

        [OutputTerminal(Direction.South)]
        public GraphicEffect Effect { get; set; } = new KaleidoscopeEffect { Color = new Color(255f, 255f, 255f, 255f) };

        [InputTerminal(Direction.North)]
        public void Rotation(float rotation)
        {
            KaleidoscopeEffect.Rotation = rotation;
        }

        [InputTerminal(Direction.West)]
        public void Color(Color data)
        {
            if (data != null)
            {
                KaleidoscopeEffect.Color = data;
            }
        }

        [InputTerminal(Direction.North)]
        public void Sides(int sides)
        {
            KaleidoscopeEffect.NumberOfSides = sides;
        }

        [InputTerminal(Direction.East)]
        public void SetShape(GraphicEffect effect)
        {
            KaleidoscopeEffect.Effect = effect;
        }
    }
}