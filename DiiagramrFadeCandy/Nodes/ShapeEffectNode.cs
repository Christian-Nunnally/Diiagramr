using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;

namespace DiiagramrFadeCandy
{
    public class ShapeEffectNode : Node
    {
        public ShapeEffectNode()
        {
            Width = 90;
            Height = 90;
            Name = "Shape Effect";
            ShapeGraphic.Visible = true;
            ShapeGraphic.X = 0.5f;
            ShapeGraphic.Y = 0.5f;
            ShapeGraphic.Width = 1.0f;
            ShapeGraphic.Height = 1.0f;
            ShapeGraphic.Thickness = 1;
            ShapeGraphic.Fill = false;
        }

        public ShapeEffect ShapeGraphic => Effect as ShapeEffect;

        [OutputTerminal(Direction.South)]
        public GraphicEffect Effect { get; set; } = new ShapeEffect();

        [InputTerminal(Direction.North)]
        public bool SetVisible
        {
            get => ShapeGraphic.Visible;
            set => ShapeGraphic.Visible = value;
        }

        [InputTerminal(Direction.West)]
        public float SetY
        {
            get => ShapeGraphic.Y;
            set => ShapeGraphic.Y = value;
        }

        [InputTerminal(Direction.West)]
        public float SetX
        {
            get => ShapeGraphic.X;
            set => ShapeGraphic.X = value;
        }

        [InputTerminal(Direction.West)]
        public bool SetFill
        {
            get => ShapeGraphic.Fill;
            set => ShapeGraphic.Fill = value;
        }

        [InputTerminal(Direction.West)]
        public float SetRotation
        {
            get => ShapeGraphic.Rotation;
            set => ShapeGraphic.Thickness = value;
        }

        [InputTerminal(Direction.East)]
        public float SetWidth
        {
            get => ShapeGraphic.Width;
            set => ShapeGraphic.Width = value;
        }

        [InputTerminal(Direction.East)]
        public float SetHeight
        {
            get => ShapeGraphic.Height;
            set => ShapeGraphic.Height = value;
        }

        [InputTerminal(Direction.East)]
        public float SetThickness
        {
            get => ShapeGraphic.Thickness;
            set => ShapeGraphic.Thickness = value;
        }

        [InputTerminal(Direction.North)]
        public Color SetColor
        {
            get => new Color(ShapeGraphic.R, ShapeGraphic.G, ShapeGraphic.B, ShapeGraphic.A);
            set
            {
                if (value == null)
                {
                    return;
                }

                ShapeGraphic.R = value.R;
                ShapeGraphic.G = value.G;
                ShapeGraphic.B = value.B;
                ShapeGraphic.A = value.A;
            }
        }

        public void PickRectangle()
        {
            ShapeGraphic.Mode = Shape.Rectangle;
        }

        public void PickRoundedRectangle()
        {
            ShapeGraphic.Mode = Shape.RoundedRectangle;
        }

        public void PickEllipse()
        {
            ShapeGraphic.Mode = Shape.Ellipse;
        }
    }
}