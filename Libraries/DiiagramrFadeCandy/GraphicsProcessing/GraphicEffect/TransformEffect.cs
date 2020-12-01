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
    public class TransformEffect : GraphicEffect
    {
        public GraphicEffect Effect { get; set; }

        public float Rotation { get; set; } = 0;

        public float ScaleX { get; set; } = 1;

        public float ScaleY { get; set; } = 1;

        public float ShearX { get; set; } = 0;

        public float ShearY { get; set; } = 0;

        public float OffsetX { get; set; } = 0;

        public float OffsetY { get; set; } = 0;

        public override void Draw(RenderTarget target)
        {
            var smallerSize = Math.Min(target.Size.Width, target.Size.Height);
            var targetWidth = target.Size.Width / 2;
            var targetHeight = target.Size.Height / 2;
            var oldMatrix = target.Transform;
            var centerPoint = new PointF(targetWidth, targetHeight);
            var newMatrix = new Matrix(oldMatrix.M11, oldMatrix.M12, oldMatrix.M21, oldMatrix.M22, oldMatrix.M31, oldMatrix.M32);
            newMatrix.Translate(OffsetX * smallerSize, OffsetY * smallerSize);
            newMatrix.Scale(ScaleX, ScaleY);
            newMatrix.Shear(ShearX, ShearY);
            newMatrix.RotateAt(Rotation, centerPoint);
            var newMatrixElements = newMatrix.Elements;

            target.Transform = new RawMatrix3x2(newMatrixElements[0], newMatrixElements[1], newMatrixElements[2], newMatrixElements[3], newMatrixElements[4], newMatrixElements[5]);
            Effect?.Draw(target);
            target.Transform = oldMatrix;
        }
    }
}