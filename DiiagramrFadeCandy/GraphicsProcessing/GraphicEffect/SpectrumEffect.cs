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

        private int _iteration = 0;

        [DataMember]
        public float[] SpectrumData { get; set; }

        [DataMember]
        public Color Color { get; set; }

        [DataMember]
        public float BarWidthScale { get; set; } = 1f;

        [DataMember]
        public bool SpectrographMode { get; set; } = true;

        public override void Draw(RenderTarget target)
        {
            if (SpectrumData == null || SpectrumData.Length == 0)
            {
                return;
            }
            var targetWidth = target.Size.Width;
            var targetHeight = target.Size.Height;

            var totalWidthPerBar = targetWidth / SpectrumData.Length;
            var barWidth = totalWidthPerBar * BarWidthScale;

            if (SpectrographMode)
            {
                var rowHeight = 1;
                var maxRows = (int)Math.Floor(targetHeight / rowHeight);
                var black = new RawColor4(0, 0, 0, 1);
                var blackBrush = new SolidColorBrush(target, black);
                var left = 0f;
                var top = (_iteration + 1) * rowHeight;
                var right = targetWidth;
                var bottom = _iteration * rowHeight;
                target.FillRectangle(new RawRectangleF(left, top, right, bottom), blackBrush);

                for (int i = 0; i < SpectrumData.Length; i++)
                {
                    if (SpectrumData[i] > _maxValue && !float.IsInfinity(SpectrumData[i]))
                    {
                        _maxValue = SpectrumData[i];
                    }
                    var amplitude = SpectrumData[i] / _maxValue;
                    var brush = new SolidColorBrush(target, new RawColor4(Color.R, Color.G, Color.B, amplitude * amplitude));
                    left = i * totalWidthPerBar;
                    top = (_iteration + 1) * rowHeight;
                    right = left + barWidth;
                    bottom = _iteration * rowHeight;
                    var rectangle = new RawRectangleF(left, top, right, bottom);
                    target.FillRectangle(rectangle, brush);
                }
                _iteration = (_iteration + 1) % maxRows;
            }
            else
            {
                for (int i = 0; i < SpectrumData.Length; i++)
                {
                    if (SpectrumData[i] > _maxValue && !float.IsInfinity(SpectrumData[i]))
                    {
                        _maxValue = SpectrumData[i];
                    }
                    var brush = new SolidColorBrush(target, new RawColor4(Color.R, Color.G, Color.B, Color.A));
                    var left = i * totalWidthPerBar;
                    var top = targetHeight;
                    var right = left + barWidth;
                    var bottom = targetHeight - (targetHeight / _maxValue * SpectrumData[i]);
                    var rectangle = new RawRectangleF(left, top, right, bottom);
                    target.FillRectangle(rectangle, brush);
                }
            }
        }
    }
}