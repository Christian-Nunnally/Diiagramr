using DiiagramrAPI.Diagram;

namespace DiiagramrFadeCandy
{
    public class ShapeEffectNodeViewModel : Node
    {
        public TypedTerminal<bool> VisibleTerminal { get; private set; }
        public TypedTerminal<bool> FillTerminal { get; private set; }
        public TypedTerminal<float> XTerminal { get; private set; }
        public TypedTerminal<float> YTerminal { get; private set; }
        public TypedTerminal<float> RotationTerminal { get; private set; }
        public TypedTerminal<float> ThicknessTerminal { get; private set; }
        public TypedTerminal<float> WidthTerminal { get; private set; }
        public TypedTerminal<float> HeightTerminal { get; private set; }
        public TypedTerminal<GraphicEffect> EffectTerminal { get; private set; }
        public TypedTerminal<Color> ColorTerminal { get; private set; }

        public ShapeEffect ShapeGraphic { get; set; } = new ShapeEffect();

        protected override void SetupNode(NodeSetup setup)
        {
            setup.NodeSize(90, 90);
            setup.NodeName("Shape Effect");

            VisibleTerminal = setup.InputTerminal<bool>("Visible", Direction.North);
            VisibleTerminal.DataChanged += v => ShapeGraphic.Visible = v;
            VisibleTerminal.Data = true;
            VisibleTerminal = setup.InputTerminal<bool>("Fill", Direction.North);
            VisibleTerminal.DataChanged += f => ShapeGraphic.Fill = f;
            VisibleTerminal.Data = true;

            XTerminal = setup.InputTerminal<float>("X", Direction.West);
            XTerminal.Data = 0.5f;
            XTerminal.DataChanged += x => ShapeGraphic.X = x;
            YTerminal = setup.InputTerminal<float>("Y", Direction.West);
            YTerminal.Data = 0.5f;
            YTerminal.DataChanged += y => ShapeGraphic.Y = y;
            RotationTerminal = setup.InputTerminal<float>("Degrees Rotation", Direction.East);
            RotationTerminal.Data = 0.0f;
            RotationTerminal.DataChanged += r => ShapeGraphic.Rotation = r;
            ThicknessTerminal = setup.InputTerminal<float>("Thickness", Direction.West);
            ThicknessTerminal.Data = 0.5f;
            ThicknessTerminal.DataChanged += t => ShapeGraphic.Thickness = t;
            WidthTerminal = setup.InputTerminal<float>("Width", Direction.East);
            WidthTerminal.Data = .9f;
            WidthTerminal.DataChanged += w => ShapeGraphic.Width = w;
            HeightTerminal = setup.InputTerminal<float>("Height", Direction.East);
            HeightTerminal.Data = .9f;
            HeightTerminal.DataChanged += h => ShapeGraphic.Height = h;

            ColorTerminal = setup.InputTerminal<Color>("Color", Direction.North);
            ColorTerminal.DataChanged += ColorTerminalTerminalDataChanged;
            EffectTerminal = setup.OutputTerminal<GraphicEffect>("Effect", Direction.South);
            EffectTerminal.Data = ShapeGraphic;
        }

        private void ColorTerminalTerminalDataChanged(Color color)
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
