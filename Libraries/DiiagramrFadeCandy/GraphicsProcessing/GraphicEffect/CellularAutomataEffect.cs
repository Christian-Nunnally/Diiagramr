using SharpDX.Direct2D1;
using System;
using System.Runtime.Serialization;

namespace DiiagramrFadeCandy
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class CellularAutomataEffect : GraphicEffect
    {
        private Random _random;
        private uint[] _intPixelData;
        private int _colorChangeMode = 3;

        public CellularAutomataEffect()
        {
            Randomize();
        }

        [DataMember]
        public float[] SpectrumData { get; set; }

        [DataMember]
        public Color Color { get; set; }

        [DataMember]
        public int Width { get; set; } = 128;

        [DataMember]
        public int Height { get; set; } = 128;

        [DataMember]
        public float BarWidth { get; set; } = 1f;

        private Random Random => _random ?? (_random = new Random());
        private uint[] IntPixelData => _intPixelData ?? (_intPixelData = new uint[Width * Height]);

        public override void Draw(RenderTarget target)
        {
            Step();
            Bitmap backBufferBitmap = new Bitmap(target, new SharpDX.Size2(Width, Height), new BitmapProperties(target.PixelFormat));
            backBufferBitmap.CopyFromMemory(IntPixelData, Width * 4);
            target.DrawBitmap(backBufferBitmap, 1.0f, BitmapInterpolationMode.Linear);
        }

        public void Randomize()
        {
            for (int row = 0; row < Height; row++)
            {
                for (int col = 0; col < Width; col++)
                {
                    IntPixelData[row * Width + col] = GetRandomColor();
                }
            }
            _colorChangeMode = Random.Next(10);
        }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
        }

        private uint GetRandomColor()
        {
            byte[] bytes = new byte[4];
            Random.NextBytes(bytes);
            return BitConverter.ToUInt32(bytes, 0);
        }

        private void Step()
        {
            for (int row = 0; row < Height; row++)
            {
                for (int col = 0; col < Width; col++)
                {
                    if (Random.NextDouble() > 0.5)
                    {
                        var currentIndex = row * Width + col;
                        var currentColor = IntPixelData[currentIndex];
                        var newIndex = PickRandomCardinalIndex(currentIndex);
                        if (ShouldChangeColor(IntPixelData[newIndex], currentColor))
                        {
                            IntPixelData[newIndex] = currentColor;
                        }
                    }
                }
            }
        }

        private bool ShouldChangeColor(uint oldColor, uint newColor)
        {
            return _colorChangeMode switch
            {
                0 => oldColor < newColor,
                1 => oldColor > newColor,
                2 => (oldColor & 255) > (newColor & 255),
                3 => (oldColor & 255) < (newColor & 255),
                4 => (oldColor & 65280) > (newColor & 65280),
                5 => (oldColor & 65280) < (newColor & 65280),
                6 => (oldColor & 16711680) > (newColor & 16711680),
                7 => (oldColor & 16711680) < (newColor & 16711680),
                8 => (oldColor & 4278190080) > (newColor & 4278190080),
                9 => (oldColor & 4278190080) < (newColor & 4278190080),
                _ => true,
            };
        }

        private int PickRandomCardinalIndex(int startingIndex)
        {
            return (Random.Next(0, 4)) switch
            {
                0 => (startingIndex + Width) % IntPixelData.Length,
                1 => (startingIndex + IntPixelData.Length - Width) % IntPixelData.Length,
                2 => (startingIndex + 1) % IntPixelData.Length,
                3 => (startingIndex + IntPixelData.Length - 1) % IntPixelData.Length,
                _ => throw new InvalidOperationException(),
            };
        }
    }
}