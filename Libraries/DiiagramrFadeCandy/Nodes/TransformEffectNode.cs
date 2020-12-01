using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;

namespace DiiagramrFadeCandy.Nodes
{
    [Help("Tanslates, rotates, and/or skews the render target before drawing the effect wired in to this node.")]
    public class TransformEffectNode : Node
    {
        public TransformEffectNode()
        {
            Name = "Transform Effect";
            Width = 60;
            Height = 60;
        }

        [Help("Sets the effect to transform.")]
        [InputTerminal(Direction.North)]
        public GraphicEffect Effect { get => TransformedEffect.Effect; set => TransformedEffect.Effect = value; }

        [Help("Sets the amount to rotate the effect in degrees.")]
        [InputTerminal(Direction.West)]
        public float Rotation { get => TransformedEffect.Rotation; set => TransformedEffect.Rotation = value; }

        [Help("Sets the amount to scale the effect in the X direction.")]
        [InputTerminal(Direction.West)]
        public float ScaleX { get => TransformedEffect.ScaleX; set => TransformedEffect.ScaleX = value; }

        [Help("Sets the amount to scale the effect in the Y direction.")]
        [InputTerminal(Direction.West)]
        public float ScaleY { get => TransformedEffect.ScaleY; set => TransformedEffect.ScaleY = value; }

        [Help("Sets the amount to shear the effect in the X direction.")]
        [InputTerminal(Direction.East)]
        public float ShearX { get => TransformedEffect.ShearX; set => TransformedEffect.ShearX = value; }

        [Help("Sets the amount to shear the effect in the Y direction.")]
        [InputTerminal(Direction.East)]
        public float ShearY { get => TransformedEffect.ShearY; set => TransformedEffect.ShearY = value; }

        [Help("Sets the amount to offset the effect in the X direction.")]
        [InputTerminal(Direction.North)]
        public float OffsetX { get => TransformedEffect.OffsetX; set => TransformedEffect.OffsetX = value; }

        [Help("Sets the amount to offset the effect in the Y direction.")]
        [InputTerminal(Direction.North)]
        public float OffsetY { get => TransformedEffect.OffsetY; set => TransformedEffect.OffsetY = value; }

        [Help("Sets the effect to transform.")]
        [OutputTerminal(Direction.South)]
        public TransformEffect TransformedEffect { get; set; } = new TransformEffect();
    }
}