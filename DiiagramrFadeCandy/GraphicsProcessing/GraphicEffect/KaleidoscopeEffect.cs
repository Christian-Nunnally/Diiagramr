using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using System;
using System.Numerics;
using System.Runtime.Serialization;

namespace DiiagramrFadeCandy
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class KaleidoscopeEffect : GraphicEffect
    {
        [DataMember]
        public byte[] SpectrumData { get; set; }

        [DataMember]
        public Color Color { get; set; }

        public GraphicEffect Effect { get; set; }

        public float Rotation { get; set; } = 30;

        public int NumberOfSides { get; set; } = 8;

        public override void Draw(RenderTarget target)
        {
            var targetWidth = target.Size.Width;
            var targetHeight = target.Size.Height;

            var oldMatrix = target.Transform;
            var centerVector = new Vector2(targetWidth / 2, targetHeight / 2);

            for (int i = 0; i < NumberOfSides; i++)
            {
                var radians = (Rotation + (360 / NumberOfSides * i)) * ((float)Math.PI / 180f);
                var matrix = Matrix3x2.CreateRotation(radians, centerVector);
                target.Transform = new RawMatrix3x2(matrix.M11, matrix.M12, matrix.M21, matrix.M22, matrix.M31, matrix.M32);

                Effect?.Draw(target);
            }

            target.Transform = oldMatrix;
        }
    }
}