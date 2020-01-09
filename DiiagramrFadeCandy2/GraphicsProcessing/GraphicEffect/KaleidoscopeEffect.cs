using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using System;
using System.Numerics;
using System.Runtime.Serialization;

namespace DiiagramrFadeCandy
{
    [Serializable]
    public class KaleidoscopeEffect : GraphicEffect
    {
        [DataMember]
        public byte[] SpectrumData { get; set; }

        [DataMember]
        public Color Color { get; set; }

        public int NumberOfSides { get; private set; } = 8;

        public override void Draw(RenderTarget target)
        {
            var targetWidth = target.Size.Width;
            var targetHeight = target.Size.Height;

            var oldMatrix = target.Transform;
            var centerVector = new Vector2(targetWidth / 2, targetHeight / 2);

            for (int i = 0; i < NumberOfSides; i++)
            {
                var radians = 360 / NumberOfSides * i * (float)Math.PI / 180f;
                var matrix = Matrix3x2.CreateRotation(radians, centerVector);
                target.Transform = new RawMatrix3x2(matrix.M11, matrix.M12, matrix.M21, matrix.M22, matrix.M31, matrix.M32);

                var barWidth = 6;
                var brush = new SolidColorBrush(target, new RawColor4(Color.R / i, Color.G, (1 / NumberOfSides) * i, Color.A));
                var left = i * barWidth;
                var top = targetHeight;
                var right = i * barWidth + barWidth;
                var bottom = 0;
                var rectangle = new RawRectangleF(left, top, right, bottom);
                target.FillRectangle(rectangle, brush);
            }

            target.Transform = oldMatrix;
        }
    }
}