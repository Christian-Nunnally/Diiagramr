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
        }

        public ShapeEffect ShapeGraphic => Effect as ShapeEffect;

        [OutputTerminal(Direction.South)]
        public GraphicEffect Effect { get; set; } = new ShapeEffect();

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

        [InputTerminal(Direction.North)]
        public void SetVisible(bool visible)
        {
            ShapeGraphic.Visible = visible;
        }

        [InputTerminal(Direction.West)]
        public void SetY(float y)
        {
            ShapeGraphic.Y = y;
        }

        [InputTerminal(Direction.West)]
        public void SetX(float x)
        {
            ShapeGraphic.X = x;
        }

        [InputTerminal(Direction.West)]
        public void SetFill(bool fill)
        {
            ShapeGraphic.Fill = fill;
        }

        [InputTerminal(Direction.West)]
        public void SetRotation(float rotation)
        {
            ShapeGraphic.Rotation = rotation;
        }

        [InputTerminal(Direction.East)]
        public void SetWidth(float width)
        {
            ShapeGraphic.Width = width;
        }

        [InputTerminal(Direction.East)]
        public void SetHeight(float height)
        {
            ShapeGraphic.Height = height;
        }

        [InputTerminal(Direction.East)]
        public void SetThickness(float thickness)
        {
            ShapeGraphic.Thickness = thickness;
        }

        [InputTerminal(Direction.North)]
        public void SetColor(Color color)
        {
            if (color == null)
            {
                return;
            }

            ShapeGraphic.R = color.R;
            ShapeGraphic.G = color.G;
            ShapeGraphic.B = color.B;
            ShapeGraphic.A = color.A;
        }
    }
}