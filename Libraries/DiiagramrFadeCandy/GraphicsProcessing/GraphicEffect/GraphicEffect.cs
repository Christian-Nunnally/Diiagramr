using DiiagramrAPI.Editor.Diagrams;
using SharpDX.Direct2D1;
using System;
using System.Runtime.Serialization;

namespace DiiagramrFadeCandy
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class GraphicEffect : IWireableType
    {
        public virtual void Draw(RenderTarget target)
        {
        }

        public System.Drawing.Color GetTypeColor()
        {
            return System.Drawing.Color.FromArgb(255, 99, 77, 99);
        }

        public override string ToString()
        {
            return "Effect";
        }
    }
}