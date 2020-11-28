using DiiagramrAPI.Editor.Diagrams;
using DiiagramrAPI.Editor.Interactors;
using DiiagramrModel;

namespace DiiagramrFadeCandy
{
    [HideFromNodeSelector]
    public class AnimatedShapeEffectNode : Node
    {
        public AnimatedShapeEffectNode()
        {
            Width = 30;
            Height = 30;
            Name = "Animated Shape Effect";
        }

        public AnimatedShapeEffect AnimatedShapeEffect => Effect as AnimatedShapeEffect;

        [OutputTerminal(Direction.South)]
        public GraphicEffect Effect { get; set; } = new AnimatedShapeEffect { Color = new Color(255f, 255f, 255f, 255f) };

        [InputTerminal(Direction.East)]
        public int Speed
        {
            set => AnimatedShapeEffect.FrameDelay = value;
            get => AnimatedShapeEffect.FrameDelay;
        }

        [InputTerminal(Direction.West)]
        public Color Color
        {
            get => AnimatedShapeEffect.Color;
            set
            {
                if (value != null)
                {
                    AnimatedShapeEffect.Color = value;
                }
            }
        }

        [InputTerminal(Direction.North)]
        public bool Trigger
        {
            set => AnimatedShapeEffect.ResetPoints();
            get => true;
        }
    }
}