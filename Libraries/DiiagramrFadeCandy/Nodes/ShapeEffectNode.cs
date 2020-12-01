using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;

namespace DiiagramrFadeCandy
{
    [Help("Basic primitive shape effect.")]
    public class ShapeEffectNode : Node
    {
        public ShapeEffectNode()
        {
            Width = 60;
            Height = 60;
            Name = "Shape Effect";
            ShapeGraphic.Visible = true;
            ShapeGraphic.Thickness = .1f;
            ShapeGraphic.Fill = false;
        }

        public ShapeEffect ShapeGraphic => Effect as ShapeEffect;

        [NodeSetting]
        [OutputTerminal(Direction.South)]
        [Help("Outputs a reference to the effect.")]
        public GraphicEffect Effect { get; set; } = new ShapeEffect();

        [NodeSetting]
        [InputTerminal(Direction.North)]
        [Help("Shows or hides the shape.")]
        public bool SetVisible
        {
            get => ShapeGraphic.Visible;
            set => ShapeGraphic.Visible = value;
        }

        [NodeSetting]
        [InputTerminal(Direction.North)]
        [Help("Shows or hides the background of the shape.")]
        public bool SetFill
        {
            get => ShapeGraphic.Fill;
            set => ShapeGraphic.Fill = value;
        }

        [NodeSetting]
        [InputTerminal(Direction.East)]
        [Help("Changes the radius of the corners on the shape.")]
        public float CornerRadius
        {
            get => ShapeGraphic.CornerRadius;
            set => ShapeGraphic.CornerRadius = value;
        }

        [NodeSetting]
        [InputTerminal(Direction.East)]
        [Help("Changes the thickness of the shape border.")]
        public float SetThickness
        {
            get => ShapeGraphic.Thickness;
            set => ShapeGraphic.Thickness = value;
        }

        [NodeSetting]
        [InputTerminal(Direction.West)]
        [Help("Changes the color of the shape.")]
        public Color SetColor
        {
            get => new Color(ShapeGraphic.R, ShapeGraphic.G, ShapeGraphic.B, ShapeGraphic.A);
            set
            {
                if (value == null) return;
                ShapeGraphic.R = value.R;
                ShapeGraphic.G = value.G;
                ShapeGraphic.B = value.B;
                ShapeGraphic.A = value.A;
            }
        }
    }
}