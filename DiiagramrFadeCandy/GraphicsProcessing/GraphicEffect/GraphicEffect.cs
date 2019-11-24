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

        public System.Windows.Media.Color GetTypeColor()
        {
            return new System.Windows.Media.Color() { R = 85, G = 85, B = 150, A = 255 };
        }
    }
}