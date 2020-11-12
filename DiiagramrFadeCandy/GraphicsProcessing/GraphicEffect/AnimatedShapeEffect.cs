using DiiagramrFadeCandy.Nodes;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace DiiagramrFadeCandy
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class AnimatedShapeEffect : GraphicEffect
    {
        [OptionalField]
        private readonly List<RawVector2> _points = new List<RawVector2>();

        [OptionalField]
        private int _framePoint = 0;

        [OptionalField]
        private int _frameDelayCounter = 0;

        public AnimatedShapeEffect()
        {
            _points.Add(new RawVector2(0, 0));
            _points.Add(new RawVector2(8, 1));
            _points.Add(new RawVector2(16, 16));
            _points.Add(new RawVector2(32, 16));
            _points.Add(new RawVector2(32, 32));
            _points.Add(new RawVector2(32, 16));
            _points.Add(new RawVector2(0, 0));
        }

        [DataMember]
        public float[] SpectrumData { get; set; }

        [DataMember]
        public Color Color { get; set; }

        [DataMember]
        public float BarWidth { get; set; } = 1f;

        [DataMember]
        public int FrameDelay { get; set; } = 32;

        public override void Draw(RenderTarget target)
        {
            SolidColorBrush brush = new SolidColorBrush(target, Color.RawColor);

            var pathGeometry = new PathGeometry(target.Factory);
            var geometrySink = pathGeometry.Open();
            if (_points.Count > 0)
            {
                geometrySink.BeginFigure(_points.First(), FigureBegin.Filled);
                for (int i = 1; i < _points.Count && i < _framePoint; i++)
                {
                    geometrySink.AddLine(_points[i]);
                }
                geometrySink.EndFigure(FigureEnd.Open);
            }
            geometrySink.Close();
            // target.DrawGeometry(pathGeometry, brush);

            target.DrawGeometry(new GeometryNode().PathGeometry, brush);

            // important. turn pathGeometry into nodes
            // pathGeometry.Combine
            pathGeometry.Dispose();
            brush.Dispose();
            _frameDelayCounter++;
            if (_frameDelayCounter >= FrameDelay)
            {
                _framePoint++;
                _frameDelayCounter = 0;
            }
        }

        internal void ResetPoints()
        {
            _framePoint = 0;
        }
    }
}