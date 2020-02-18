using DiiagramrAPI.Editor.Diagrams;
using SharpDX.Direct2D1;
using System;

namespace DiiagramrFadeCandy
{
    [Serializable]
    public class GraphicEffect : IWireableType
    {
        public virtual void Draw(RenderTarget target)
        {
        }

        public System.Drawing.Color GetTypeColor()
        {
            return System.Drawing.Color.FromArgb(255, 99, 77, 99);
        }
    }
}