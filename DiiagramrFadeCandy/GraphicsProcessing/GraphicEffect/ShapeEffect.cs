using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using System;
using System.Runtime.Serialization;

namespace DiiagramrFadeCandy
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class ShapeEffect : GraphicEffect
    {
        [DataMember]
        public bool Visible { get; set; }

        [DataMember]
        public float Rotation { get; set; }

        [DataMember]
        public float Thickness { get; set; }

        [DataMember]
        public float CornerRadius { get; set; }

        [DataMember]
        public float R { get; set; } = 0.5f;

        [DataMember]
        public float G { get; set; } = 0.5f;

        [DataMember]
        public float B { get; set; } = 0.9f;

        [DataMember]
        public float A { get; set; } = 1.0f;

        [DataMember]
        public bool Fill { get; set; } = true;

        public override void Draw(RenderTarget target)
        {
            if (!Visible)
            {
                return;
            }

            using var brush = new SolidColorBrush(target, new RawColor4(R, G, B, A));

            var smallerSideLength = Math.Min(target.Size.Width, target.Size.Height);
            var xRadius = smallerSideLength / 2.0f;
            var yRadius = smallerSideLength / 2.0f;
            var xCenter = target.Size.Width / 2f;
            var yCenter = target.Size.Height / 2f;
            var left = xCenter - xRadius;
            var top = yCenter - yRadius;
            var right = xCenter + xRadius;
            var bottom = yCenter + yRadius;
            var rectangle = new RawRectangleF(left, top, right, bottom);
            var xCornerRadius = xRadius * CornerRadius;
            var yCornerRadius = yRadius * CornerRadius;
            if (Fill)
            {
                var roundedRectangle = new RoundedRectangle { Rect = rectangle, RadiusX = xCornerRadius, RadiusY = yCornerRadius };
                target.FillRoundedRectangle(roundedRectangle, brush);
            }
            else
            {
                var roundedRectangle = new RoundedRectangle { Rect = rectangle, RadiusX = xCornerRadius, RadiusY = yCornerRadius };
                target.DrawRoundedRectangle(roundedRectangle, brush, Thickness * smallerSideLength);
            }
        }
    }
}