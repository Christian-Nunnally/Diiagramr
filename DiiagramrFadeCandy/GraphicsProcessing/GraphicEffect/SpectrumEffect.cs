using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using System;
using System.Runtime.Serialization;

namespace DiiagramrFadeCandy
{
    [Serializable]
    public class SpectrumEffect : GraphicEffect
    {
        private float _maxValue;

        [DataMember]
        public int[] SpectrumData { get; set; }

        [DataMember]
        public Color Color { get; set; }

        [DataMember]
        public float BarWidth { get; set; }

        public override void Draw(RenderTarget target)
        {
            if (SpectrumData == null)
            {
                return;
            }
            var targetWidth = target.Size.Width;
            var targetHeight = target.Size.Height;

            var totalWidthPerBar = targetWidth / SpectrumData.Length;
            var barWidth = totalWidthPerBar * BarWidth;
            var remainingWidth = totalWidthPerBar - barWidth;
            var halfRemainingWidth = remainingWidth / 2;
            for (int i = 0; i < SpectrumData.Length; i++)
            {
                if (SpectrumData[i] > _maxValue)
                {
                    _maxValue = SpectrumData[i];
                }
                var brush = new SolidColorBrush(target, new RawColor4(Color.R, Color.G, Color.B, Color.A));
                var left = i * totalWidthPerBar;
                var top = targetHeight;
                var right = left + halfRemainingWidth;
                var bottom = targetHeight - (targetHeight / _maxValue * SpectrumData[i]);
                var rectangle = new RawRectangleF(left, top, right, bottom);
                target.FillRectangle(rectangle, brush);
            }
        }
    }
}