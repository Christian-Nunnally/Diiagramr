using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
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

        public int NumberOfSides { get; set; } = 8;

        public override void Draw(RenderTarget target)
        {
            var targetWidth = target.Size.Width;
            var targetHeight = target.Size.Height;

            var oldMatrix = target.Transform;
            var newMatrix = new Matrix(oldMatrix.M11, oldMatrix.M12, oldMatrix.M21, oldMatrix.M22, oldMatrix.M31, oldMatrix.M32);

            var centerPoint = new PointF(targetWidth / 2, targetHeight / 2);
            var degrees = 360f / NumberOfSides;

            for (int i = 0; i < NumberOfSides; i++)
            {
                newMatrix.RotateAt(degrees, centerPoint);
                var newMatrixElements = newMatrix.Elements;
                target.Transform = new RawMatrix3x2(newMatrixElements[0], newMatrixElements[1], newMatrixElements[2], newMatrixElements[3], newMatrixElements[4], newMatrixElements[5]);
                Effect?.Draw(target);
            }

            target.Transform = oldMatrix;
        }
    }
}