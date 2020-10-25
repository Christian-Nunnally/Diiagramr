using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;

namespace DiiagramrFadeCandy
{
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
        public void Speed(int frameDelay)
        {
            AnimatedShapeEffect.FrameDelay = frameDelay;
        }

        [InputTerminal(Direction.West)]
        public void Color(Color data)
        {
            if (data != null)
            {
                AnimatedShapeEffect.Color = data;
            }
        }

        [InputTerminal(Direction.North)]
        public void Trigger(bool _)
        {
            AnimatedShapeEffect.ResetPoints();
        }
    }
}