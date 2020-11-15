using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using System;
using System.Runtime.Serialization;

namespace DiiagramrFadeCandy
{
    [Serializable]
    [DataContract(IsReference = true)]
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

        [DataMember]
        public float ScaleExponent { get; set; } = 5f;

        [DataMember]
        public float MaxValueDecayRate { get; set; } = .99999f;

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
            _maxValue *= MaxValueDecayRate;

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
                var amplitudes = new float[SpectrumData.Length];
                target.FillRectangle(new RawRectangleF(left, top, right, bottom), blackBrush);

                for (int i = 0; i < SpectrumData.Length; i++)
                {
                    _maxValue *= MaxValueDecayRate;
                    if (SpectrumData[i] > _maxValue && !float.IsInfinity(SpectrumData[i]))
                    {
                        _maxValue = SpectrumData[i];
                    }
                    amplitudes[i] = (float)Math.Pow(SpectrumData[i] / _maxValue, ScaleExponent);
                    var brush = new SolidColorBrush(target, new RawColor4(Color.R, Color.G, Color.B, amplitudes[i]));
                    left = i * totalWidthPerBar;
                    top = (_iteration + 1) * rowHeight;
                    right = left + barWidth;
                    bottom = _iteration * rowHeight;
                    var rectangle = new RawRectangleF(left, top, right, bottom);
                    target.FillRectangle(rectangle, brush);
                }

                //var bassTraceRange = Math.Min(SpectrumData.Length, 10);
                //var newIndexOfBassTraceRange = 0.0f;
                //var totalBrightness = 0.0f;
                //for (int i = 0; i < bassTraceRange; i++)
                //{
                //    newIndexOfBassTraceRange += amplitudes[i] * i;
                //    totalBrightness += amplitudes[i];
                //}
                //totalBrightness = totalBrightness == 0 ? 1 : totalBrightness;
                //newIndexOfBassTraceRange /= totalBrightness;
                //_indexOfBassTraceRange += _bassTraceVelocity * (newIndexOfBassTraceRange - _indexOfBassTraceRange);

                //if (totalBrightness > 2)
                //{
                //    var brightness = Math.Min(totalBrightness, 1);
                //    var bassBarWidth = totalBrightness / 2;
                //    var brush2 = new SolidColorBrush(target, new RawColor4(1, 0, .8f, brightness));
                //    left = (_indexOfBassTraceRange * totalWidthPerBar) - (bassBarWidth / 2);
                //    top = (_iteration + 1) * rowHeight;
                //    right = left + bassBarWidth;
                //    bottom = _iteration * rowHeight;
                //    var rectangle2 = new RawRectangleF(left, top, right, bottom);
                //    target.FillRectangle(rectangle2, brush2);
                //}

                _iteration = (_iteration + 1) % maxRows;
            }
            else
            {
                for (int i = 0; i < SpectrumData.Length; i++)
                {
                    var data = (float)Math.Log(SpectrumData[i] + 1);
                    if (data > _maxValue && !float.IsInfinity(data))
                    {
                        _maxValue = data;
                    }
                    var brush = new SolidColorBrush(target, new RawColor4(Color.R, Color.G, Color.B, Color.A));
                    var left = i * totalWidthPerBar;
                    var top = targetHeight;
                    var right = left + barWidth;
                    var bottom = targetHeight - (targetHeight * (data / _maxValue));
                    var rectangle = new RawRectangleF(left, top, right, bottom);
                    target.FillRectangle(rectangle, brush);
                }
            }
        }
    }
}